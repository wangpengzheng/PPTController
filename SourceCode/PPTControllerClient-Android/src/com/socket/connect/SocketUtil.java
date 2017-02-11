package com.socket.connect;

import java.io.ByteArrayOutputStream;
import java.io.InputStream;
import java.io.OutputStream;
import java.net.InetAddress;
import java.net.InetSocketAddress;
import java.net.Socket;

import android.graphics.Bitmap;
import android.graphics.BitmapFactory;
import android.os.Handler;
import android.os.Looper;
import android.os.Message;
import com.example.pptcontroller.Execution;
import com.pptcontroller.global.SysApplication;
import com.pptcontroller.util.ExceptionHandler;

public class SocketUtil implements Runnable {

	private static final int SERVER_PORTAL = 13001;
	private static final int TIMEOUT = 3000;
	public static final int WIFI_CONNECTION_TYPE = 1;
	public static final int BLUETOOTH_CONNECTION_TYPE = 2;

	private String ipAddress;
	private OutputStream ops;
	private InputStream ips;
	private Socket socket;
	private int connectionType = WIFI_CONNECTION_TYPE;
	public Execution context;

	public SocketUtil(String ipAddress) {
		super();
		this.ipAddress = ipAddress;
	}

	public void send(String data) throws Exception {
		if (data == null) {
			throw new IllegalArgumentException("Data is null");
		}
		try {

			if (getConnectionType() == WIFI_CONNECTION_TYPE) {
				InetAddress remoteAddress = InetAddress.getByName(ipAddress);
				InetSocketAddress remoteSocketAddress = new InetSocketAddress(
						remoteAddress, SERVER_PORTAL);
				socket = new Socket();
				socket.connect(remoteSocketAddress, TIMEOUT);
				setOps(socket.getOutputStream());

				byte[] buffer = (data + "<EOF>").getBytes("UTF-8");

				getOps().write(buffer);

				getOps().flush();

				doStop();

				if (SysApplication.getInstance().isEnablePreview == true) {
					new Thread(getHostScreenThread).start();
				}

			} else if (getConnectionType() == BLUETOOTH_CONNECTION_TYPE) {
				byte[] buffer = (data + "<EOF>").getBytes("UTF-8");

				getOps().write(buffer);

				getOps().flush();

				if (SysApplication.getInstance().isEnablePreview == true) {
					new Thread(getHostScreenThread).start();
				}
			}
		} catch (Exception e) {
			throw e;
		}
	}

	public void doStop() {
		try {
			if (getOps() != null) {
				getOps().close();
				setOps(null);
			}
			if (socket != null) {
				socket.close();
				socket = null;
			}
		} catch (Exception ex) {
			ExceptionHandler.handleWarning(ex);
		}
	}

	@Override
	public void run() {
		String command = Thread.currentThread().getName();
		try {
			this.send(command);
		} catch (Exception e) {
			ExceptionHandler.handleWarning(e);
		}
	}

	public int getConnectionType() {
		return connectionType;
	}

	public void setConnectionType(int connectionType) {
		this.connectionType = connectionType;
	}

	public OutputStream getOps() {
		return ops;
	}

	public void setOps(OutputStream ops) {
		this.ops = ops;
	}

	public InputStream getIps() {
		return ips;
	}

	public void setIps(InputStream ips) {
		this.ips = ips;
	}

	private Runnable getHostScreenThread = new Runnable() {
		@Override
		public void run() {

			Looper.prepare();

			try {
				InputStream localIps = null;
				OutputStream localOps = null;
				if (getConnectionType() == WIFI_CONNECTION_TYPE) {

					Socket getHostScreenSocket = null;

					InetAddress remoteAddress = InetAddress
							.getByName(ipAddress);
					InetSocketAddress remoteSocketAddress = new InetSocketAddress(
							remoteAddress, SERVER_PORTAL);
					getHostScreenSocket = new Socket();
					getHostScreenSocket.connect(remoteSocketAddress, TIMEOUT);

					localOps = getHostScreenSocket.getOutputStream();
					localIps = getHostScreenSocket.getInputStream();

					GetHostScreen(localOps, localIps);

					getHostScreenSocket.close();

				} else if (getConnectionType() == BLUETOOTH_CONNECTION_TYPE) {
					localOps = getOps();
					localIps = getIps();
					GetHostScreen(localOps, localIps);
				}
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
	};

	private void GetHostScreen(OutputStream localOps, InputStream localIps) {
		synchronized (SocketUtil.class) {
			try {
				String cmd = "GetScreen" + "<EOF>";
				localOps.write(cmd.getBytes("UTF-8"));
				localOps.flush();

				// byte[] readByte = new byte[8192];
				byte[] readByte = new byte[65536];
				int readCount = -1;
				int totolReadCount = 0;

				ByteArrayOutputStream bytestream = new ByteArrayOutputStream();
				byte[] len = new byte[8];
				localIps.read(len);
				int count = bytesToInt(len);
				while (true) {
					readCount = localIps.read(readByte);
					totolReadCount += readCount;
					bytestream.write(readByte, 0, readCount);
					if (totolReadCount == count) {
						break;
					}
				}
				byte[] screenData = bytestream.toByteArray();
				bytestream.close();
				Bitmap screen = BitmapFactory.decodeByteArray(screenData, 0,
						screenData.length);

				if (screen != null) {
					Message msg = Message.obtain();
					msg.obj = screen;
					hanlder.sendMessage(msg);
				}
			} catch (Exception e) {
				e.printStackTrace();
			}
		}
	}

	private Handler hanlder = new Handler() {

		@Override
		public void handleMessage(Message msg) {

			super.handleMessage(msg);
			context.handleMessage(msg);

		}

	};

	private String getInfoBuff(byte[] buff, int count) {
		byte[] temp = new byte[count];
		for (int i = 0; i < count; i++) {
			temp[i] = buff[i];
		}
		return new String(temp);
	}

	public int bytesToInt(byte[] intByte) {
		int fromByte = 0;
		for (int i = 0; i < 2; i++) {
			int n = (intByte[i] < 0 ? (int) intByte[i] + 256 : (int) intByte[i]) << (8 * i);

			fromByte += n;
		}
		return fromByte;
	}
}
