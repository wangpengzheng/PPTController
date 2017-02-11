package com.pptcontroller.global;

import java.io.OutputStream;

import android.app.Application;

public class PPTController extends Application {
	
	private String hotSpotDefaultIP;
	
	private OutputStream ops_global = null;
	
	private static final String NAME = "192.168.173.1";

	@Override
	public void onCreate() {
		// TODO Auto-generated method stub
		super.onCreate();
		setDefaultHotSpotIP(NAME);
	}
	
	public String getDefaultHotSpotIP(){
		return hotSpotDefaultIP;
	}
	
	public void setDefaultHotSpotIP(String ip){
		this.hotSpotDefaultIP = ip;
	}
	
	public OutputStream getGlobalOutputStream(){
		return ops_global;
	}
	
	public void setGlobalOutputStream(OutputStream ops){
		this.ops_global = ops;
	}
	
}
