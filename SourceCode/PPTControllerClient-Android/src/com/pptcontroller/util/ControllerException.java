package com.pptcontroller.util;

public class ControllerException extends Exception 
{
	private static final long serialVersionUID = 1L;
	
	private String message;
	
	public ControllerException()
	{
		super();
	}
	
	public ControllerException(String errorMsg)
	{
		super();
		
		this.message = errorMsg;
	}

	public String toString()
	{
		return this.message;
	}
}
