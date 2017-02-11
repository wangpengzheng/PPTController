package com.pptcontroller.util;

import java.util.regex.Matcher;
import java.util.regex.Pattern;

public class InputValidator {
	private static String regexIPAddress = "\\b((?!\\d\\d\\d)\\d+|1\\d\\d|2[0-4]\\d|25[0-5])\\.((?!\\d\\d\\d)\\d+|1\\d\\d|2[0-4]\\d|25[0-5])\\.((?!\\d\\d\\d)\\d+|1\\d\\d|2[0-4]\\d|25[0-5])\\.((?!\\d\\d\\d)\\d+|1\\d\\d|2[0-4]\\d|25[0-5])\\b";
    private static String regexPort = "^\\d+$";

    public static boolean ValidateIpAddress(String ipAddress)
    {
    	Pattern pattern = Pattern.compile(regexIPAddress);
    	Matcher match = pattern.matcher(ipAddress);
        return match.matches();
    }

    public static boolean ValidatePort(String port)
    {
    	Pattern pattern = Pattern.compile(regexPort);
    	Matcher match = pattern.matcher(port);
        return match.matches();
    }
}
