package com.pptcontroller.util;

import com.example.pptcontroller.R;

/*
 *  MessageProvider is to provide the message that is used in PPT controller.
 *  We can move text string to resource files in the future. 
 */
public class MessageProvider 
{
	public static final int DefaultError = R.string.message_default;
	
	public static final int DefaultError_Args = R.string.message_default_args;
	
	public static final int Multicast_TimeOutError = R.string.message_multicast_timeout_error;

	public static final int Multicast_InitializationError = R.string.message_multicast_initialization_error_args;
	
	public static final int DIRECTCONNECT_IPERROR = R.string.message_ipconnect_error;
	
	public static final int DIRECTCONNECT_NOWIFIERROR = R.string.message_wificonnect_error;
}
