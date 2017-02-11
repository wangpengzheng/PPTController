package com.pptcontroller.util;

import java.net.DatagramPacket;
import java.net.InetAddress;
import java.net.MulticastSocket;

import android.net.wifi.WifiManager;
import android.net.wifi.WifiManager.MulticastLock;

public class NetUtil {
	private static final int MULTICAST_PORT = 7890;
	
	private static final String GROUP_IP = "224.0.0.2";
	
	MulticastLock multicastLock;
	WifiManager wifiManager;
	
	public NetUtil(WifiManager wifiManager){
		this.wifiManager = wifiManager;
	}

	public void doSendGroupMsg(String message){
		allowMulticast();
		MulticastSocket skt = null;
		
		try
		{
			skt = new MulticastSocket(MULTICAST_PORT);
			InetAddress broadcastAddress = InetAddress.getByName(GROUP_IP);
			skt.joinGroup(broadcastAddress);
			skt.setLoopbackMode(false);
			DatagramPacket sendPgk = new DatagramPacket(
					message.getBytes(),
					message.getBytes().length,
					broadcastAddress,
					MULTICAST_PORT
			);
			while(true){
				skt.send(sendPgk);
				System.out.println(sendPgk);
				Thread.sleep(2000);
			}
		}
		catch(Exception e){
			ExceptionHandler.handleWarning(e);
		}
		finally
		{
			try
			{
				skt.close();
			}
			catch(Exception e){
				ExceptionHandler.handleWarning(e);
			}
			try
			{
				multicastLock.release();
			}
			catch(Exception e){
				ExceptionHandler.handleWarning(e);
			}
		}

	}
	
	private void allowMulticast(){
		try
		{
			multicastLock=wifiManager.createMulticastLock("multicastLock");   
			multicastLock.acquire(); 
		}
		catch(Exception e){
			ExceptionHandler.handleWarning(e);
		}
	}
	
	public static void doSendUdpGroupMsg(WifiManager wifiManager, String message){
		NetUtil netUtil = new NetUtil(wifiManager);
		netUtil.doSendGroupMsg(message);
		netUtil = null;
	}
}
