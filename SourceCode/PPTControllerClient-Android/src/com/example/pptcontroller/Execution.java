package com.example.pptcontroller;

import com.example.pptcontroller.R;
import com.pptcontroller.global.SysApplication;
import com.pptcontroller.util.ExceptionHandler;
import com.pptcontroller.util.ExecutionActivityController;
import com.pptcontroller.util.GestureUtil;
import com.pptcontroller.util.GestureUtil.Screen;
//import com.socket.connect.SocketConnection;
import com.socket.connect.SocketUtil;

import android.os.Bundle;
import android.os.Handler;
import android.os.Message;
import android.app.Activity;
import android.app.AlertDialog;
import android.app.Dialog;
import android.content.DialogInterface;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.Bitmap;
import android.view.GestureDetector;
import android.view.GestureDetector.OnGestureListener;
import android.view.Menu;
import android.view.MenuItem;
import android.view.MotionEvent;
import android.view.View;
import android.view.View.OnTouchListener;
import android.widget.ImageView;
import android.widget.RelativeLayout;

public class Execution extends Activity implements OnTouchListener,
		OnGestureListener {

	private GestureDetector mGestureDetector;

	private SocketUtil socketUtil;
	private String ipAddress;

	private ImageView iv_up;
	private ImageView iv_down;
	private ImageView iv_left;
	private ImageView iv_right;
	private ImageView previewImg;

	private static final int GESTURE_UP = 0;
	private static final int GESTURE_DOWN = 1;
	private static final int GESTURE_LEFT = 2;
	private static final int GESTURE_RIGHT = 3;

	private Screen screen;

	@SuppressWarnings("deprecation")
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		setContentView(R.layout.activity_execution);
		SysApplication.getInstance().addActivity(this);
		setTheme(android.R.style.Theme_Black);

		Intent intent = getIntent();

		int connectionType = intent.getIntExtra("ConnectionType",
				SocketUtil.WIFI_CONNECTION_TYPE);

		if (connectionType == SocketUtil.WIFI_CONNECTION_TYPE) {
			ipAddress = intent.getStringExtra("ipAddress");
			socketUtil = new SocketUtil(ipAddress);
			socketUtil.setConnectionType(SocketUtil.WIFI_CONNECTION_TYPE);
		} else if (connectionType == SocketUtil.BLUETOOTH_CONNECTION_TYPE) {
			socketUtil = new SocketUtil(null);
			socketUtil.setConnectionType(SocketUtil.BLUETOOTH_CONNECTION_TYPE);
			socketUtil.setOps(ExecutionActivityController.getInstance()
					.getOutputStream());
			socketUtil.setIps(ExecutionActivityController.getInstance()
					.getInputStream());
		}
		socketUtil.context = this;
		screen = GestureUtil.getScreenPix(this);
		mGestureDetector = new GestureDetector(this);
		RelativeLayout touchLayout = (RelativeLayout) findViewById(R.id.touchLayout);
		touchLayout.setOnTouchListener(this);
		touchLayout.setLongClickable(true);

		iv_up = (ImageView) findViewById(R.id.touchUpImg);
		iv_down = (ImageView) findViewById(R.id.touchDownImg);
		iv_left = (ImageView) findViewById(R.id.touchLeftImg);
		iv_right = (ImageView) findViewById(R.id.touchRightImg);
		previewImg = (ImageView) findViewById(R.id.previewImg);

		iv_up.setOnClickListener(listener);
		iv_down.setOnClickListener(listener);
		iv_left.setOnClickListener(listener);
		iv_right.setOnClickListener(listener);
	}

	private View.OnClickListener listener = new View.OnClickListener() {

		@Override
		public void onClick(View v) {
			// TODO Auto-generated method stub
			switch (v.getId()) {
			case R.id.touchUpImg:
				doResult(GESTURE_UP);
				break;
			case R.id.touchDownImg:
				doResult(GESTURE_DOWN);
				break;
			case R.id.touchLeftImg:
				doResult(GESTURE_LEFT);
				break;
			case R.id.touchRightImg:
				doResult(GESTURE_RIGHT);
				break;
			default:
				break;
			}
		}
	};

	@Override
	public boolean onCreateOptionsMenu(Menu menu) {
		// Inflate the menu; this adds items to the action bar if it is present.
		getMenuInflater().inflate(R.menu.execution, menu);
		return true;
	}

	@Override
	public boolean onOptionsItemSelected(MenuItem item) {
		// TODO Auto-generated method stub
		try {
			switch (item.getItemId()) {
			case R.id.action_start:
				Thread threadStart = new Thread(socketUtil, "START");
				threadStart.start();
				break;
			case R.id.action_fullscreen:
				Thread threadFullScreen = new Thread(socketUtil, "FULL");
				threadFullScreen.start();
				break;
			case R.id.action_exit:
				Thread threadExit = new Thread(socketUtil, "ESC");
				threadExit.start();
				break;
			default:
				break;
			}
		} catch (Exception e) {
			ExceptionHandler.handleWarning(e);
		}
		return super.onOptionsItemSelected(item);
	}

	@Override
	public boolean onDown(MotionEvent arg0) {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public boolean onFling(MotionEvent e1, MotionEvent e2, float velocityX,
			float velocityY) {
		// TODO Auto-generated method stub

		float x = e2.getX() - e1.getX();
		float y = e2.getY() - e1.getY();
		float x_limit = screen.widthPixels / 5;
		float y_limit = screen.heightPixels / 5;

		float x_abs = Math.abs(x);
		float y_abs = Math.abs(y);

		if (x_abs >= y_abs) {
			if (x > x_limit || x < -x_limit) {
				if (x > 0) {
					doResult(GESTURE_RIGHT);
				} else if (x <= 0) {
					doResult(GESTURE_LEFT);
				}
			}
		} else {
			if (y > y_limit || y < -y_limit) {
				if (y > 0) {
					doResult(GESTURE_DOWN);
				} else if (y <= 0) {
					doResult(GESTURE_UP);
				}
			}
		}
		return true;
	}

	private void doResult(int result) {
		// TODO Auto-generated method stub
		switch (result) {
		case GESTURE_UP:
			Thread upThread = new Thread(socketUtil, "HOME");
			upThread.start();
			break;
		case GESTURE_DOWN:
			Thread downThread = new Thread(socketUtil, "END");
			downThread.start();
			break;
		case GESTURE_LEFT:
			Thread leftThread = new Thread(socketUtil, "UP");
			leftThread.start();
			break;
		case GESTURE_RIGHT:
			Thread rightThread = new Thread(socketUtil, "DOWN");
			rightThread.start();
			break;
		default:
			break;
		}
	}

	@Override
	public void onLongPress(MotionEvent arg0) {
		// TODO Auto-generated method stub

	}

	@Override
	public boolean onScroll(MotionEvent e1, MotionEvent e2, float distanceX,
			float distanceY) {
		// TODO Auto-generated method stub
		return false;
	}

	@Override
	public void onShowPress(MotionEvent arg0) {
		// TODO Auto-generated method stub

	}

	@Override
	public boolean onSingleTapUp(MotionEvent e) {
		// TODO Auto-generated method stub
		float x = e.getX();
		float middleX = screen.widthPixels / 2;
		if (x > middleX) {
			doResult(GESTURE_RIGHT);
		} else if (x < middleX) {
			doResult(GESTURE_LEFT);
		}
		return true;
	}

	@Override
	public boolean onTouch(View v, MotionEvent event) {
		// TODO Auto-generated method stub
		return mGestureDetector.onTouchEvent(event);
	}

	public void handleMessage(Message msg) {
		if (msg.obj != null) {
			Bitmap preview = (Bitmap) msg.obj;
			ExecutionActivityController.getInstance().setPreviewBitmap(preview);
			previewImg.setImageBitmap(preview);
		}

	}

	@Override
	public void onConfigurationChanged(Configuration newConfig) {
		super.onConfigurationChanged(newConfig);
		Bitmap preview = ExecutionActivityController.getInstance()
				.getPreviewBitmap();
		if (preview != null) {
			previewImg.setImageBitmap(preview);
		}
	}
}
