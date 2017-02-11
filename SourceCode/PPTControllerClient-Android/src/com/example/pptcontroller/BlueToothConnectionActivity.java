package com.example.pptcontroller;

import java.io.IOException;
import java.io.InputStream;
import java.io.OutputStream;
import java.util.Set;
import java.util.UUID;

import com.example.pptcontroller.R;
import com.pptcontroller.global.SysApplication;
import com.pptcontroller.util.DialogHelper;
import com.pptcontroller.util.ExceptionHandler;
import com.pptcontroller.util.ExecutionActivityController;
import com.socket.connect.SocketUtil;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.provider.Settings;
import android.annotation.SuppressLint;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.bluetooth.BluetoothAdapter;
import android.bluetooth.BluetoothDevice;
import android.bluetooth.BluetoothSocket;
import android.content.BroadcastReceiver;
import android.content.Context;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.IntentFilter;
import android.graphics.Color;
import android.util.Log;
import android.view.Gravity;
import android.view.LayoutInflater;
import android.view.Menu;
import android.view.View;
import android.view.ViewGroup;
import android.view.View.OnClickListener;
import android.widget.AdapterView;
import android.widget.BaseAdapter;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.AdapterView.OnItemClickListener;
import android.widget.Toast;

@SuppressLint("HandlerLeak")
public class BlueToothConnectionActivity extends Activity {

	ListView listView;
	ImageButton blueToothImageButton;
	ImageButton refreshButton;
	Button connectButton;
	BluetoothAdapter mBluetoothAdapter = BluetoothAdapter.getDefaultAdapter();
	BluetoothListViewAdapter pairedDevicesAdapter;
	String logTag = "BlueTooth";
	private int cur_pos = 0;
	String message;

	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_blue_tooth_connection);
		SysApplication.getInstance().addActivity(this);

		listView = (ListView) findViewById(R.id.computerListView);
		listView.setOnItemClickListener(ItemListener);

		blueToothImageButton = (ImageButton) findViewById(R.id.bluetoothButton);
		blueToothImageButton.setOnClickListener(BlueToothImageButtonOnClick);

		refreshButton = (ImageButton) findViewById(R.id.refreshImageButton);
		refreshButton.setOnClickListener(RefreshButtonOnClick);

		connectButton = (Button) findViewById(R.id.connectButton);
		connectButton.setOnClickListener(ConnectButtonOnClick);

		listView.setChoiceMode(ListView.CHOICE_MODE_SINGLE);
		ShowPairedDevices();

		if (!mBluetoothAdapter.isEnabled()) {

			DialogHelper.Alert(this, getString(R.string.text_bluetooth),
					getString(R.string.dialog_bluetooth_enable),
					getString(R.string.dialog_button_ok),
					new DialogInterface.OnClickListener() {

						public void onClick(DialogInterface dialog, int which) {
							Intent intent = new Intent(
									Settings.ACTION_BLUETOOTH_SETTINGS);
							startActivity(intent);
						}
					});
		}

		IntentFilter filter = new IntentFilter(BluetoothDevice.ACTION_FOUND);
		registerReceiver(mReceiver, filter);
		filter = new IntentFilter(BluetoothAdapter.ACTION_DISCOVERY_FINISHED);
		registerReceiver(mReceiver, filter);
	}

	private OnClickListener ConnectButtonOnClick = new OnClickListener() {
		@Override
		public void onClick(View view) {

			if (mBluetoothAdapter.getBondedDevices().size() > 0) {
				BluetoothDevice device = getBluetoothDevice(cur_pos);
				/*
				 * showMessage("Connecting to " + device.getName() +
				 * " and pay attention to the request from host");
				 */
				showMessage(String.format(
						getString(R.string.message_connect_bluetooth_format),
						device.getName()));
				try {
					if (ExecutionActivityController.getInstance().getSocket() != null) {
						ExecutionActivityController.getInstance().getSocket()
								.close();
					}
				} catch (IOException e) {
					ExceptionHandler.handleWarning(e);
				}
				openSocket(device);
			}
		}
	};

	private OnClickListener RefreshButtonOnClick = new OnClickListener() {
		@Override
		public void onClick(View view) {
			ShowPairedDevices();
		}
	};

	private void ShowPairedDevices() {
		try {
			Set<BluetoothDevice> pairedDevices = mBluetoothAdapter
					.getBondedDevices();
			pairedDevicesAdapter = new BluetoothListViewAdapter(
					BlueToothConnectionActivity.this, pairedDevices);
		} catch (Exception ex) {

		}

		listView.setAdapter(pairedDevicesAdapter);
	}

	private OnClickListener BlueToothImageButtonOnClick = new OnClickListener() {
		@Override
		public void onClick(View view) {
			Intent intent = new Intent(Settings.ACTION_BLUETOOTH_SETTINGS);
			startActivity(intent);
		}
	};

	private BroadcastReceiver mReceiver = new BroadcastReceiver() {

		@Override
		public void onReceive(Context context, Intent intent) {
			String action = intent.getAction();
			if (BluetoothDevice.ACTION_FOUND.equals(action)) {
				BluetoothDevice device = intent
						.getParcelableExtra(BluetoothDevice.EXTRA_DEVICE);

				showMessage(getString(R.string.message_find_bluetooth)
						+ device.getName() + device.getAddress());

			} else if (BluetoothAdapter.ACTION_DISCOVERY_FINISHED
					.equals(action)) {
				showMessage(getString(R.string.message_find_bluetooth_done));
			}
		}
	};

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.blue_tooth_connection, menu);
		return true;
	}

	private OnItemClickListener ItemListener = new OnItemClickListener() {
		@Override
		public void onItemClick(AdapterView<?> parent, View view, int position,// parentÂ¾Ã�ÃŠÃ�?ListViewÂ£Â¬viewÂ±Ã­ÃŠÂ¾ItemÃŠÃ“Ã�Â¼Â£Â¬positionÂ±Ã­ÃŠÂ¾ÃŠÃ½Â¾Ã�Ã�?Ã·Ã’Ã�?
				long id) {
			cur_pos = position;
			pairedDevicesAdapter.notifyDataSetChanged();
		}
	};

	private BluetoothDevice getBluetoothDevice(int position) {
		Set<BluetoothDevice> pairedDevices = mBluetoothAdapter
				.getBondedDevices();
		BluetoothDevice[] devices = pairedDevices
				.toArray(new BluetoothDevice[pairedDevices.size()]);
		return devices[position];
	}

	private Handler hanlder = new Handler() {
		@Override
		public void handleMessage(Message msg) {

			super.handleMessage(msg);
			popupMessagebox(message);
		}
	};

	private void popupMessagebox(String message) {

		try {
			Dialog alertDialog = new AlertDialog.Builder(this)
					.setCancelable(false)
					.setMessage(message)
					.setIcon(R.drawable.ic_launcher)
					.setPositiveButton(getString(R.string.dialog_button_ok),
							new DialogInterface.OnClickListener() {

								@Override
								public void onClick(DialogInterface dialog,
										int which) {
									dialog.cancel();
								}
							}).create();
			alertDialog.show();
		} catch (Exception ex) {

		}
	}

	private void showMessage(String str) {

		Toast toast = Toast.makeText(getApplicationContext(), str,
				Toast.LENGTH_LONG);
		toast.setGravity(Gravity.CENTER, 0, 0);
		toast.show();
	}

	private class BluetoothListViewAdapter extends BaseAdapter {
		private LayoutInflater inflater;
		private Set<BluetoothDevice> dataSource;
		private BluetoothDevice[] devices = null;

		public BluetoothListViewAdapter(Context context,
				Set<BluetoothDevice> source) {
			inflater = (LayoutInflater) context
					.getSystemService(Context.LAYOUT_INFLATER_SERVICE);
			dataSource = source;
			devices = dataSource
					.toArray(new BluetoothDevice[dataSource.size()]);
		}

		@Override
		public int getCount() {
			return dataSource.size();
		}

		@Override
		public Object getItem(int position) {
			return devices[position];
		}

		@Override
		public long getItemId(int position) {
			return position;
		}

		@Override
		public View getView(int position, View convertView, ViewGroup parent) {
			convertView = inflater.inflate(R.layout.bluetooth_listview_item,
					null, false);
			TextView tv = (TextView) convertView
					.findViewById(R.id.bluetoothItemTextView);
			BluetoothDevice device = (BluetoothDevice) getItem(position);
			tv.setText(device.getName() + " : " + device.getAddress());
			if (position == cur_pos) {
				convertView.setBackgroundColor(Color.LTGRAY);
				tv.setTextColor(Color.RED);
			}
			return convertView;
		}
	}

	private void openSocket(BluetoothDevice bluetoothDevice) {
		try {

			final ConnectRunnable connector = new ConnectRunnable(
					bluetoothDevice);

			new Thread(connector).start();

		} catch (IOException ex) {
			Log.d(logTag, "Could not open bluetooth socket", ex);

		} catch (NoSuchMethodException ex) {

		}
	}

	private void closeSocket(BluetoothSocket openSocket) {
		try {
			if (openSocket != null) {
				openSocket.close();
			}
		} catch (IOException ex) {
			Log.d(logTag, "Could not close exisiting socket", ex);
		}
	}

	private class ConnectRunnable implements Runnable {
		public ConnectRunnable(BluetoothDevice device) throws IOException,
				NoSuchMethodException {
			mBluetoothAdapter.cancelDiscovery();
			ExecutionActivityController
					.getInstance()
					.setSocket(
							device.createRfcommSocketToServiceRecord(UUID
									.fromString("00001101-0000-1000-8000-00805F9B34FB")));
		}

		public void run() {
			try {

				ExecutionActivityController.getInstance().getSocket().connect();

				OutputStream output = ExecutionActivityController.getInstance()
						.getSocket().getOutputStream();
				ExecutionActivityController.getInstance().setOutputStream(
						output);

				InputStream input = ExecutionActivityController.getInstance()
						.getSocket().getInputStream();
				ExecutionActivityController.getInstance().setInputStream(input);

				Intent intent = new Intent();
				intent.setClass(BlueToothConnectionActivity.this,
						Execution.class);
				intent.putExtra("ConnectionType",
						SocketUtil.BLUETOOTH_CONNECTION_TYPE);
				startActivity(intent);

			} catch (IOException connectException) {
				Log.d(logTag, "Could not connect to socket", connectException);
				message = getString(R.string.message_connect_bluetooth_fail);
				Message msg = Message.obtain();
				hanlder.sendMessage(msg);
				closeSocket(ExecutionActivityController.getInstance()
						.getSocket());
				return;
			}

		}
	}

	@Override
	public void onDestroy() {

		closeSocket(ExecutionActivityController.getInstance().getSocket());
		this.unregisterReceiver(mReceiver);
		super.onDestroy();
	}
}
