package com.example.pptcontroller;

import android.app.Activity;
import android.content.Intent;
import android.net.Uri;
import android.os.Bundle;
import android.text.Editable;
import android.text.TextWatcher;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.CompoundButton.OnCheckedChangeListener;
import android.widget.EditText;

import com.example.pptcontroller.R;
import com.pptcontroller.global.PPTController;
import com.pptcontroller.global.SysApplication;
import com.pptcontroller.util.ExecutionActivityController;
import com.pptcontroller.util.VibratorUtil;

public class SetupActivity extends Activity {

	EditText hotspotDefaultIP;
	Button resetBtn;
	Button visitBtn;
	Button voteBtn;
	CheckBox vibrateChbx;
	CheckBox previewChbx;
	
	PPTController app;
	
	String homeURI;
	boolean isVibrated = true;
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_setup);
		SysApplication.getInstance().addActivity(this);
		
		app = (PPTController)getApplication();
		homeURI = this.getString(R.string.userguide_link);
		
		hotspotDefaultIP = (EditText)findViewById(R.id.edTextDefaultIP);
		hotspotDefaultIP.setText(app.getDefaultHotSpotIP());		
		hotspotDefaultIP.addTextChangedListener(defaultHotSpotIPWatcher);
		
		resetBtn = (Button)findViewById(R.id.btnReset);
		resetBtn.setOnClickListener(resetBtnOnClickListener);
		
		visitBtn = (Button)findViewById(R.id.btnVisit);
		visitBtn.setOnClickListener(visitBtnClickListener);
		
		voteBtn = (Button)findViewById(R.id.btnVote);
		voteBtn.setOnClickListener(voteBtnClickListener);
		
		vibrateChbx = (CheckBox)findViewById(R.id.chbVibrate);
		vibrateChbx.setOnCheckedChangeListener(vibrateChangedListener);
		
		previewChbx = (CheckBox)findViewById(R.id.chbPrevious);
		previewChbx.setOnCheckedChangeListener(previewChangedListener);
	}
	
	private OnCheckedChangeListener vibrateChangedListener = new OnCheckedChangeListener(){

		@Override
		public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
			// TODO Auto-generated method stub
			isVibrated = isChecked;
		}
	};
	
	private OnCheckedChangeListener previewChangedListener = new OnCheckedChangeListener() {
		
		@Override
		public void onCheckedChanged(CompoundButton buttonView, boolean isChecked) {
			// TODO Auto-generated method stub
			SysApplication.getInstance().isEnablePreview = isChecked;
			if(isChecked==false)
			{
				ExecutionActivityController.getInstance().setPreviewBitmap(null);
			}
		}
	};
	
	private OnClickListener voteBtnClickListener = new OnClickListener(){

		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			doVibrated(isVibrated);
//			Intent intent = new Intent(Intent.ACTION_VIEW);
//			intent.setData(Uri.parse("market://details?id=com.example.pptcontroller_1"));
//			startActivity(intent);
		}
	};
	
	private OnClickListener visitBtnClickListener = new OnClickListener(){

		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			doVibrated(isVibrated);
			Uri uri = Uri.parse(homeURI);
			Intent intent = new Intent(Intent.ACTION_VIEW, uri);
			startActivity(intent);
		}
	};
	
	private OnClickListener resetBtnOnClickListener = new OnClickListener(){

		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			doVibrated(isVibrated);
			hotspotDefaultIP.setText("192.168.173.1");
		}
	};
	
	private TextWatcher defaultHotSpotIPWatcher = new TextWatcher(){

		@Override
		public void afterTextChanged(Editable arg0) {
			// TODO Auto-generated method stub
			app.setDefaultHotSpotIP(arg0.toString());
		}

		@Override
		public void beforeTextChanged(CharSequence arg0, int arg1, int arg2,
				int arg3) {
			// TODO Auto-generated method stub
			
		}

		@Override
		public void onTextChanged(CharSequence arg0, int arg1, int arg2,
				int arg3) {
			// TODO Auto-generated method stub
			
		}
		
	};
	
	private void doVibrated(boolean isChecked){
		if(isChecked){
			VibratorUtil.Vibrate(SetupActivity.this, 100);
		}
	}
}
