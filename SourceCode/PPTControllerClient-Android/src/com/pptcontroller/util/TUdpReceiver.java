package com.pptcontroller.util;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;
import java.util.ArrayList;
import java.util.List;

import com.example.pptcontroller.R;
import com.example.pptcontroller.R.string;
import com.pptcontroller.global.PPTController;
import com.pptcontroller.global.SysApplication;

import android.annotation.SuppressLint;
import android.content.Context;
import android.content.DialogInterface;
import android.net.wifi.WifiManager;
import android.net.wifi.WifiManager.MulticastLock;
import android.os.Handler;
import android.os.Message;

public class TUdpReceiver implements Runnable {
	private static final String GROUP_IP = "224.0.0.2";

	private static final int MULTICAST_PORT = 13002;

	private static final int MAXLENGTH = 1024;

	// Timeout period(MS)
	private static final int TIMEOUT = 1500;

	private MulticastSocket socket = null;

	private WifiManager wifiManager = null;

	private MulticastLock multicastLock = null;

	private InetAddress broadcastAddress = null;

	private boolean isActived = false;

	private Context context;

	public TUdpReceiver(WifiManager wifiManager, Context context) {
		this.wifiManager = wifiManager;
		this.context = context;
	}

	private void doInit() throws Exception {
		try {
			if (this.wifiManager != null) {
				this.multicastLock = wifiManager
						.createMulticastLock("multicastLock");
				this.multicastLock.acquire();

				this.broadcastAddress = InetAddress.getByName(GROUP_IP);

				this.socket = new MulticastSocket(MULTICAST_PORT);
				this.socket.joinGroup(broadcastAddress);
				this.socket.setLoopbackMode(false);
				this.socket.setSoTimeout(TIMEOUT);
			}
		} catch (Exception ex) {
			ExceptionHandler.handleWarning(ex);
			throw new ControllerException(String.format(context
					.getString(MessageProvider.Multicast_InitializationError),
					ex.toString()));
		}
	}

	public void doStart() throws ControllerException {
		if (!this.isActived) {
			try {
				this.doInit();
				this.isActived = true;
			} catch (Exception ex) {
				ExceptionHandler.handleWarning(ex);
			}
		}
	}

	@SuppressLint("HandlerLeak")
	private Handler mHandler = new Handler() {
		public void handleMessage(Message msg) {
			if (SysApplication.getInstance().isWifiPageActive) {
				switch (msg.what) {
				case ExceptionHandler.CONTROLLER_MULTICAST_TIMEOUT:
					DialogHelper.Alert(context, context
							.getString(R.string.dialog_wifi_multicast), context
							.getString(MessageProvider.Multicast_TimeOutError),
							context.getString(R.string.dialog_button_ok),
							new DialogInterface.OnClickListener() {
								@Override
								public void onClick(DialogInterface dialog,
										int which) {
									// TODO Auto-generated method stub
									dialog.cancel();
								};
							});
					break;
				default:
					break;
				}
			}
		};
	};

	public void doStop() {
		if (this.isActived) {
			try {
				if (this.socket != null) {
					this.isActived = false;
					this.socket.leaveGroup(broadcastAddress);
					this.socket.close();
					this.socket = null;
				}
			} catch (Exception ex) {
				ExceptionHandler.handleWarning(ex);
			} finally {
				this.multicastLock.release();
			}
		}
	}

	@Override
	public void run() {
		try {
			this.doStart();
		} catch (ControllerException e) {
			// TODO Auto-generated catch block
			e.printStackTrace();
		}
		if (this.isActived) {
			try {
				byte[] inBuff = new byte[MAXLENGTH];
				DatagramPacket inPacket = new DatagramPacket(inBuff,
						inBuff.length);

				this.socket.setTimeToLive(2);
				this.socket.receive(inPacket);

				this.processReceivedData(inBuff, inPacket.getAddress()
						.toString());
				this.isActived = false;
			} catch (Exception ex) {
				mHandler.sendEmptyMessage(ExceptionHandler.CONTROLLER_MULTICAST_TIMEOUT);
			} finally {
				this.doStop();
			}
		}
	}

	private void processReceivedData(byte[] inBuff, String ipAddress) {
		String data = inBuff.toString().trim();

		String[] dataArray = data.split("|");

		boolean isAdded = false;
		String computerName = "";
		if (dataArray.length == 2 && dataArray[0] == "F") {
			computerName = dataArray[1];

			for (HostInfo hostInfo : SysApplication.getInstance().hList) {
				if (hostInfo.computerName == computerName) {
					isAdded = true;
				}
			}
		}

		if (!isAdded) {
			HostInfo host = new HostInfo(computerName, ipAddress);
			SysApplication.getInstance().hList.add(host);
		}
	}
}