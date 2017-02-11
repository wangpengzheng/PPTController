package com.pptcontroller.util;

import java.util.LinkedList;
import java.util.List;


public class ExceptionHandler 
{
	public static final int CONTROLLER_WARNING = 0;
	public static final int CONTROLLER_ERROR = 1;
	public static final int CONTROLLER_MULTICAST_TIMEOUT = 3;
	
	private static List<Exception> exceptionList = new LinkedList<Exception>();
	
	public static void handleError(Exception e)
	{
		// Not implemented. Print stack trace first.
		e.printStackTrace();
	}
	
	public static void handleError(ControllerException e)
	{
		// Not implemented. Print stack trace first.
		e.printStackTrace();
	}
	
	public static void handleWarning(Exception e)
	{
		// Not implemented. Print stack trace first.
		e.printStackTrace();
	}
	
	public static void handleWarning(ControllerException e)
	{
		// Not implemented. Print stack trace first.
		e.printStackTrace();
	}
	
	public static boolean pushExceptions(Exception e)
	{
		try
		{
			exceptionList.add(e);
		}
		catch(Exception ex)
		{
			return false;
		}
		
		return true;
	}
	
	public static void dealExceptionList()
	{
		// Not implemented
	}
}
