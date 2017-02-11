package com.example.pptcontroller;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.net.wifi.WifiManager;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.provider.Settings;
import android.view.View;
import android.view.View.OnClickListener;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ImageButton;
import android.widget.ProgressBar;
import android.widget.TextView;

import com.example.pptcontroller.R;
import com.pptcontroller.global.PPTController;
import com.pptcontroller.global.SysApplication;
import com.pptcontroller.util.DialogHelper;
import com.pptcontroller.util.ExceptionHandler;
import com.pptcontroller.util.InputValidator;
import com.pptcontroller.util.MessageProvider;
import com.socket.connect.SocketUtil;

public class WifiIPDirectActivity extends Activity {

	Button connect;
	EditText ipText;
	ProgressBar pgb;
	TextView titleText;
	ImageButton wifiSetup;

	private boolean isConnecting = false;
	private Socket socketClient = null;
	private Thread threadClient = null;
	private InputStream ips;
	private OutputStream ops;

	private String mode;
	private String ipAddress;
	private String receiveMsg;
	private PPTController app;
	private static final int SHOW_DIALOG = 1;
	private static final String SUCCESS = "Connect_Successfully";
	private static final String INITMESSAGE = "Connect<EOF>";

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_wifi_ipdirect);
		SysApplication.getInstance().addActivity(this);

		app = (PPTController) getApplication();

		Intent intent = getIntent();
		mode = intent.getStringExtra("mode");
		
		connect = (Button) findViewById(R.id.connectIpButton);
		connect.setOnClickListener(connectIpClickListener);

		wifiSetup = (ImageButton) findViewById(R.id.wifiSetupButton);
		wifiSetup.setOnClickListener(wifiSetupClickListener);

		ipText = (EditText) findViewById(R.id.ipAddressEditText);
		pgb = (ProgressBar) findViewById(R.id.widget110);

		titleText = (TextView) findViewById(R.id.titleText);
		
		if (mode.equalsIgnoreCase("hotspot")) {
			ipText.setText(app.getDefaultHotSpotIP());
			titleText.setText(R.string.text_hotspot);
			this.setTitle(getTitle(), getString(R.string.text_hotspot));
		} else {
			titleText.setText(R.string.text_wifi);
			this.setTitle(getTitle(), getString(R.string.text_wifi));
		}

		WifiManager wifiManager = (WifiManager) this
				.getSystemService(Context.WIFI_SERVICE);

		if (!wifiManager.isWifiEnabled()) {

			DialogHelper.Alert(this, getString(R.string.text_wifi),
					getString(R.string.dialog_wifi_enable),
					getString(R.string.dialog_button_ok),
					new DialogInterface.OnClickListener() {

						public void onClick(DialogInterface dialog, int which) {
							Intent intent = new Intent(
									Settings.ACTION_WIFI_SETTINGS);
							startActivity(intent);
						}
					});
		}
	}

	private OnClickListener wifiSetupClickListener = new OnClickListener() {
		@Override
		public void onClick(View view) {
			Intent wifiSettingsIntent = new Intent(
					Settings.ACTION_WIFI_SETTINGS);
			startActivity(wifiSettingsIntent);
		}
	};

	private OnClickListener connectIpClickListener = new OnClickListener() {

		@Override
		public void onClick(View v) {
			pgb.setVisibility(View.VISIBLE);

			if (isConnecting) {
				isConnecting = false;
				try {
					if (socketClient != null) {
						socketClient.close();
						socketClient = null;
					}
				} catch (IOException e) {
					ExceptionHandler.handleWarning(e);
				}
			} else {
				isConnecting = true;

				threadClient = new Thread(runnable);
				threadClient.start();
			}

		}
	};

	private Runnable runnable = new Runnable() {
		public void run() {

			Looper.prepare();
			ipAddress = ipText.getText().toString();
			if (!InputValidator.ValidateIpAddress(ipAddress)) {
				Message sendmsg = Message.obtain();
				sendmsg.what = SHOW_DIALOG;
				hanlder.sendMessage(sendmsg);

			}

			try {
				InetAddress remoteAddress = InetAddress.getByName(ipAddress);

				InetSocketAddress remoteSocketAddress = new InetSocketAddress(
						remoteAddress, 13001);
				socketClient = new Socket();
				socketClient.connect(remoteSocketAddress, 3000);

				ops = socketClient.getOutputStream();

				String connectMsg = INITMESSAGE;

				ops.write(connectMsg.getBytes("UTF-8"));

				ops.flush();

				ips = socketClient.getInputStream();

			} catch (Exception e) {
				Message msg = Message.obtain();
				msg.what = 2;
				hanlder.sendMessage(msg);
				return;
			}

			byte[] buffer = new byte[1024];
			int count = 0;
			while (isConnecting) {
				try {
					count = ips.read(buffer);
					if (count > 0) {
						String message = getInfoBuff(buffer, count);

						if (message.equals(SUCCESS)) {
							navigateToExecution();
						} else {
							Message msg = Message.obtain();
							msg.what = 2;
							hanlder.sendMessage(msg);
						}
					} else {
						Message msg = Message.obtain();
						msg.what = 2;
						hanlder.sendMessage(msg);
					}
				} catch (Exception e) {
					receiveMsg = e.getMessage();
					Message msg = Message.obtain();
					msg.what = 3;
					hanlder.sendMessage(msg);
				}
			}
		}
	};

	@SuppressLint("HandlerLeak")
	private Handler hanlder = new Handler() {

		@Override
		public void handleMessage(Message msg) {

			super.handleMessage(msg);

			isConnecting = false;

			if (msg.what == SHOW_DIALOG) {
				popupMessagebox(getString(MessageProvider.DIRECTCONNECT_IPERROR));
			} else if (msg.what == 2) {
				popupMessagebox(getString(MessageProvider.DIRECTCONNECT_NOWIFIERROR));
			} else if (msg.what == 3) {
				popupMessagebox(receiveMsg);
			}

			pgb.setVisibility(View.GONE);
		}

	};

	private void popupMessagebox(String message) {

		Dialog alertDialog = new AlertDialog.Builder(this).setCancelable(false)
				.setMessage(message).setIcon(R.drawable.ic_launcher)
				.setPositiveButton(getString(R.string.dialog_button_ok), new DialogInterface.OnClickListener() {

					@Override
					public void onClick(DialogInterface dialog, int which) {
						dialog.cancel();
					}
				}).create();
		alertDialog.show();
	}

	@Override
	protected void onPause() {
		// TODO Auto-generated method stub
		super.onPause();
		pgb.setVisibility(View.GONE);
	}

	/**
	 * Navigate to Execution page.
	 */
	private void navigateToExecution() {
		isConnecting = false;
		Intent intent = new Intent();
		intent.setClass(WifiIPDirectActivity.this, Execution.class);
		intent.putExtra("ipAddress", ipAddress);
		intent.putExtra("ConnectionType", SocketUtil.WIFI_CONNECTION_TYPE);
		startActivity(intent);
	}

	private String getInfoBuff(byte[] buff, int count) {
		byte[] temp = new byte[count];
		for (int i = 0; i < count; i++) {
			temp[i] = buff[i];
		}
		return new String(temp);
	}

	private void setTitle(CharSequence title, String mode) {
		setTitle(title + " " + mode);
	}

}
