package com.example.pptcontroller;



import com.example.pptcontroller.R;
import com.pptcontroller.global.SysApplication;
import com.pptcontroller.util.DialogHelper;
import com.pptcontroller.util.UpdateManager;


import android.app.Activity;
import android.app.ProgressDialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.os.Bundle;
import android.os.Handler;
import android.text.Html;
import android.text.method.LinkMovementMethod;
import android.view.KeyEvent;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.TextView;
import android.widget.Toast;

public class MainActivity extends Activity {
	
	Button wifiButton = null;
	Button hotspotButton = null;
	Button setupButton = null;
	TextView userGuide;
	
	String uriLink;
	String uriLinkText;
	
	private UpdateManager updateMan;
	private ProgressDialog updateProgressDialog;
	
	private boolean isExit = false;
	
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_main);
		SysApplication.getInstance().addActivity(this);
		
		updateMan = new UpdateManager(MainActivity.this, appUpdateCb);
		updateMan.checkUpdate();
		
		uriLink = this.getString(R.string.userguide_link);
		uriLinkText = this.getString(R.string.userguide);
		
		wifiButton = (Button)findViewById(R.id.wifiButton);
		wifiButton.setOnClickListener(WifiButtonClickListener);
		
		hotspotButton = (Button)findViewById(R.id.hotspotButton);
		hotspotButton.setOnClickListener(HotSpotButtonClickListener);
		
		userGuide = (TextView)findViewById(R.id.userGuide);
		userGuide.setText(Html.fromHtml("<a href=\""+uriLink+"\">"+uriLinkText+"</a>"));
		userGuide.setMovementMethod(LinkMovementMethod.getInstance());
		Button blueToothButton = (Button)findViewById(R.id.bluetoothButton);
		blueToothButton.setOnClickListener(blueToothClickListener);
		
		setupButton = (Button)findViewById(R.id.setupButton);
		setupButton.setOnClickListener(SetupButtonClickListener);
	}
	
	private OnClickListener WifiButtonClickListener = new OnClickListener(){
		
		@Override
		public void onClick(View view){
			Intent intent = new Intent();
			intent.setClass(MainActivity.this, WifiConnectionActivity.class);
			startActivity(intent);
		}
	};
	
	private OnClickListener HotSpotButtonClickListener = new OnClickListener(){

		@Override
		public void onClick(View arg0) {
			// TODO Auto-generated method stub
			Intent intent = new Intent();
			intent.putExtra("mode", "HotSpot");
			intent.setClass(MainActivity.this, WifiIPDirectActivity.class);
			startActivity(intent);
		}
	};

	private OnClickListener blueToothClickListener = new OnClickListener(){
		
		@Override
		public void onClick(View view){
			Intent intent = new Intent();
			intent.setClass(MainActivity.this, BlueToothConnectionActivity.class);
			startActivity(intent);
		}
	};
	
	private OnClickListener SetupButtonClickListener = new OnClickListener(){

		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			Intent intent = new Intent();
			intent.setClass(MainActivity.this, SetupActivity.class);
			startActivity(intent);
		}
		
	};
	
	@Override
	public boolean dispatchKeyEvent(KeyEvent event) {
		// TODO Auto-generated method stub
		
		if (event.getAction() == KeyEvent.ACTION_DOWN 
				&& event.getKeyCode() == KeyEvent.KEYCODE_BACK
				&& event.getRepeatCount() == 0) {  
	        if(!isExit) {  
	            isExit = true;    
	            Toast.makeText(getApplicationContext(), R.string.exit_press_back_twice_message, Toast.LENGTH_SHORT).show();  
	            new Handler().postDelayed(new Runnable(){  
	                public void run(){  
	                    isExit = false;  
	                }  
	            }, 2000);;  
	            return false;  
	        }  
	    }  
		
		return super.dispatchKeyEvent(event);
	}
	
	UpdateManager.UpdateCallback appUpdateCb = new UpdateManager.UpdateCallback() 
	{

		public void downloadProgressChanged(int progress) {
			if (updateProgressDialog != null
					&& updateProgressDialog.isShowing()) {
				updateProgressDialog.setProgress(progress);
			}

		}

		public void downloadCompleted(Boolean sucess, CharSequence errorMsg) {
			if (updateProgressDialog != null
					&& updateProgressDialog.isShowing()) {
				updateProgressDialog.dismiss();
			}
			if (sucess) {
				updateMan.update();
			} else {
				DialogHelper.Confirm(MainActivity.this,
						R.string.dialog_error_title,
						R.string.dialog_downfailed_msg,
						R.string.dialog_downfailed_btndown,
						new DialogInterface.OnClickListener() {

							public void onClick(DialogInterface dialog,
									int which) {
								updateMan.downloadPackage();

							}
						}, R.string.dialog_downfailed_btnnext, null);
			}
		}

		public void downloadCanceled() 
		{
			// TODO Auto-generated method stub

		}

		public void checkUpdateCompleted(Boolean hasUpdate,
				CharSequence updateInfo) {
			if (hasUpdate) {
				DialogHelper.Confirm(MainActivity.this,
						getText(R.string.dialog_update_title),
						getText(R.string.dialog_update_msg).toString()
						+updateInfo+
						getText(R.string.dialog_update_msg2).toString(),
								getText(R.string.dialog_update_btnupdate),
						new DialogInterface.OnClickListener() {

							public void onClick(DialogInterface dialog,
									int which) {
								updateProgressDialog = new ProgressDialog(
										MainActivity.this);
								updateProgressDialog
										.setMessage(getText(R.string.dialog_downloading_msg));
								updateProgressDialog.setIndeterminate(false);
								updateProgressDialog
										.setProgressStyle(ProgressDialog.STYLE_HORIZONTAL);
								updateProgressDialog.setMax(100);
								updateProgressDialog.setProgress(0);
								updateProgressDialog.show();

								updateMan.downloadPackage();
							}
						},getText( R.string.dialog_update_btnnext), null);
			}

		}
	};
}
