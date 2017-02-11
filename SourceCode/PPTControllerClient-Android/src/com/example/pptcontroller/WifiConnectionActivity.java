package com.example.pptcontroller;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.util.ArrayList;
import java.util.List;
import java.util.Set;
import java.util.concurrent.CountDownLatch;

import com.example.pptcontroller.R;
import com.pptcontroller.global.SysApplication;
import com.pptcontroller.util.ControllerException;
import com.pptcontroller.util.DialogHelper;
import com.pptcontroller.util.ExceptionHandler;
import com.pptcontroller.util.HostInfo;
import com.pptcontroller.util.InputValidator;
import com.pptcontroller.util.MessageProvider;
import com.pptcontroller.util.TUdpReceiver;
import com.socket.connect.SocketUtil;

import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.bluetooth.BluetoothDevice;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.graphics.Color;
import android.net.wifi.WifiManager;
import android.net.wifi.WifiManager.MulticastLock;
import android.os.Bundle;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import android.provider.Settings;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.TextView;

public class WifiConnectionActivity extends Activity {

	ListView listView;
	ImageButton addImageButton;
	ImageButton wifiMgrButton;
	ImageButton refreshButton;
	Button connectButton;

	WifiManager wifiManager;
	MulticastLock multicastLock;

	String host;
	TUdpReceiver receiver;
	
	WifiHostListViewAdapter adapter;

	Thread main;
	Thread tFirst;
	Thread tNext;
	private int cur_pos = 0;
	
	String ipAddress;
	Socket socketClient;
	OutputStream ops;
	InputStream ips;
	String receiveMsg;
	private Thread threadClient;
	private boolean isConnecting = false;
	private static final int SHOW_DIALOG = 1;
	private static final String INITMESSAGE = "Connect<EOF>";
	private static final String SUCCESS = "Connect_Successfully";

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_wifi_connection);
		SysApplication.getInstance().addActivity(this);
		SysApplication.getInstance().isWifiPageActive = true;

		listView = (ListView) findViewById(R.id.computerListView);
		listView.setOnItemClickListener(ItemListener);

		addImageButton = (ImageButton) findViewById(R.id.addImageButton);
		addImageButton.setOnClickListener(AddButtonOnClickListener);

		wifiMgrButton = (ImageButton) findViewById(R.id.wifiImageButton);
		wifiMgrButton.setOnClickListener(WifiButtonOnClick);

		refreshButton = (ImageButton) findViewById(R.id.refreshImageButton);
		refreshButton.setOnClickListener(RefreshButtonOnClick);
		
		connectButton = (Button)findViewById(R.id.connectButton);
		connectButton.setOnClickListener(connectClickListener);

		wifiManager = (WifiManager) this.getSystemService(Context.WIFI_SERVICE);
		

		if (!wifiManager.isWifiEnabled()) {

			DialogHelper.Alert(this, getString(R.string.text_wifi),
					getString(R.string.dialog_wifi_enable), getString(R.string.dialog_button_ok),
					new DialogInterface.OnClickListener() {

						public void onClick(DialogInterface dialog, int which) {
							Intent intent = new Intent(
									Settings.ACTION_WIFI_SETTINGS);
							startActivity(intent);
						}
					});
		}
		//SysApplication.getInstance().hList.add(new HostInfo("TestData", "172.16.185.139"));
		receiver = new TUdpReceiver(wifiManager, this);
		try {
			
			main = new Thread(mRunnable); 
			main.start();
			
		} catch (Exception e) {
			ExceptionHandler.handleWarning(e);
		}
	}
	
	@SuppressLint("HandlerLeak")
	private Handler mhandler = new Handler() {
		public void handleMessage(android.os.Message msg) {
			adapter = new WifiHostListViewAdapter(WifiConnectionActivity.this, SysApplication.getInstance().hList);
			listView.setAdapter(adapter);
		};
	};
	
	private Runnable mRunnable = new Runnable() {
		
		@Override
		public void run() {
			// TODO Auto-generated method stub
			try
			{
			tFirst = new Thread(receiver);
			
			tNext = new Thread(new Runnable() {
				
				@Override
				public void run() {
					// TODO Auto-generated method stub
					
					mhandler.sendEmptyMessage(0);
				}
			});
			
			tFirst.start();
			
			tFirst.join();
			
			tNext.start();
			
			} catch (Exception ex){
				ex.printStackTrace();
			}
		}
	};

	@Override
	protected void onStop() {
		// TODO Auto-generated method stub
		super.onStop();
		SysApplication.getInstance().isWifiPageActive = false;
	}
	
	private OnClickListener connectClickListener = new OnClickListener() {
		
		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			ipAddress = SysApplication.getInstance().hList.get(cur_pos).ipAddress;
			
			//pgb.setVisibility(View.VISIBLE);

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

	private OnClickListener WifiButtonOnClick = new OnClickListener() {
		@Override
		public void onClick(View view) {
			Intent wifiSettingsIntent = new Intent(
					Settings.ACTION_WIFI_SETTINGS);
			startActivity(wifiSettingsIntent);
		}
	};

	private OnClickListener RefreshButtonOnClick = new OnClickListener() {
		@Override
		public void onClick(View view) {
			try {
				Thread t = new Thread(receiver);

				t.start();
			} catch (Exception e) {
				ExceptionHandler.handleWarning(e);
			}
		}
	};

	private OnItemClickListener ItemListener = new OnItemClickListener() {
		@Override
		public void onItemClick(AdapterView<?> parent, View view, int position,// parent就是ListView，view表示Item视图，position表示数据索引
				long id) {
			cur_pos = position;
			adapter.notifyDataSetChanged();
		}
	};

	private OnClickListener AddButtonOnClickListener = new OnClickListener() {
		@Override
		public void onClick(View view) {
			Intent intent = new Intent();
			intent.putExtra("mode", "Wifi");
			intent.setClass(WifiConnectionActivity.this,
					WifiIPDirectActivity.class);
			startActivity(intent);
		}
	};

	private class WifiHostListViewAdapter extends BaseAdapter {
		private LayoutInflater inflater;
		private List<HostInfo> dataSource;

		public WifiHostListViewAdapter(Context context,
				List<HostInfo> source) {
			inflater = (LayoutInflater) context
					.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			dataSource = source;
		}

		@Override
		public int getCount() {
			return dataSource.size();
		}

		@Override
		public Object getItem(int position) {
			return dataSource.get(position);
		}

		@Override
		public long getItemId(int position) {
			return position;
		}

		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			convertView = inflater.inflate(R.layout.listview_item, null, false);
			TextView tv = (TextView) convertView
					.findViewById(R.id.computerItemTextView);
			HostInfo host = (HostInfo) getItem(position);
			tv.setText(host.computerName);
			if (position == cur_pos) {
				convertView.setBackgroundColor(Color.LTGRAY);
				tv.setTextColor(Color.RED);
			}
			return convertView;
		}
	}
	
	private Runnable runnable = new Runnable() {
		public void run() {

			Looper.prepare();
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

			//pgb.setVisibility(View.GONE);
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
		//pgb.setVisibility(View.GONE);
	}

	/**
	 * Navigate to Execution page.
	 */
	private void navigateToExecution() {
		isConnecting = false;
		Intent intent = new Intent();
		intent.setClass(WifiConnectionActivity.this, Execution.class);
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
}
