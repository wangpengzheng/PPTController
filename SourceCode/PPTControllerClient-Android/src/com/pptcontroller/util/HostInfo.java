package com.pptcontroller.util;

public class HostInfo {
	public String computerName;
	
	public String ipAddress;
	
	public HostInfo(String computerName, String ipAddress){
		this.computerName = computerName;
		this.ipAddress = ipAddress;
	}

	private String getComputerName() {
		return computerName;
	}

	private void setComputerName(String computerName) {
		this.computerName = computerName;
	}

	private String getIpAddress() {
		return ipAddress;
	}

	private void setIpAddress(String ipAddress) {
		this.ipAddress = ipAddress;
	}
}
