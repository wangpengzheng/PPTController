package com.socket.connect;

import java.io.Serializable;
import java.net.Socket;

public class SocketConnection implements Serializable {

	/**
	 * 
	 */
	private static final long serialVersionUID = -7112312312312300L;
	
	private Socket socket;
	
	public void setSocket(Socket socket){
		this.socket = socket;
	}
	
	public Socket getSocket(){
		return socket;
	}
}
