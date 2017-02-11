package com.pptcontroller.global;

import java.util.LinkedList;
import java.util.List;

import com.pptcontroller.util.HostInfo;

import android.app.Activity;
import android.app.Application;

public class SysApplication extends Application {
	private List<Activity> mList = new LinkedList<Activity>();
	private static SysApplication instance;
	public List<HostInfo> hList = new LinkedList<HostInfo>();
	
	public boolean isWifiPageActive = false;
	
	public boolean isEnablePreview = true;
	
	private SysApplication()
	{
		
	}
	
	public synchronized static SysApplication getInstance(){
		if(null == instance){
			instance = new SysApplication();
		}
		
		return instance;
	}
	
	public void addActivity(Activity activity){
		mList.add(activity);
	}
	
	public void exit(){
		try {
			for(Activity activity:mList){
				if(activity != null){
					activity.finish();
				}
					
			}
		} catch (Exception e) {
			e.printStackTrace();
		}
		finally{
			System.exit(0);
		}
	}

	@Override
	public void onLowMemory() {
		super.onLowMemory();
		System.gc();
	}
}
