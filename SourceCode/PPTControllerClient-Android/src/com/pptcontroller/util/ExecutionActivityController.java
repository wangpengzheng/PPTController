package com.pptcontroller.util;

import java.io.InputStream;
import java.io.OutputStream;

import android.bluetooth.BluetoothSocket;
import android.graphics.Bitmap;

public class ExecutionActivityController {
	
	private static ExecutionActivityController controller=null;
	public static ExecutionActivityController getInstance()
	{
		if(controller==null)			
		{
			controller=new ExecutionActivityController();
		}
		return controller;
	}
	
	
	private ExecutionActivityController()
	{
		
	}
	
	OutputStream outputStream=null;
	public OutputStream getOutputStream() {
		return outputStream;
	}
	public void setOutputStream(OutputStream outputStream) {
		this.outputStream = outputStream;
	}
	
	private BluetoothSocket socket = null;
	public BluetoothSocket getSocket() {
		return socket;
	}


	public void setSocket(BluetoothSocket socket) {
		this.socket = socket;
	}
	
	InputStream inputStream=null;
	public InputStream getInputStream() {
		return inputStream;
	}


	public void setInputStream(InputStream inputStream) {
		this.inputStream = inputStream;
	}
	
	Bitmap previewBitmap;
	public Bitmap getPreviewBitmap() {
		return previewBitmap;
	}


	public void setPreviewBitmap(Bitmap previewBitmap) {
		this.previewBitmap = previewBitmap;
	}
	
}
