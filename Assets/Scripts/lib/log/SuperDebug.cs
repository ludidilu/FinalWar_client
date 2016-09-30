using UnityEngine;
using System.Collections;
using System;

public class SuperDebug{

	public static void Log(object _str){
		#if !LOG_DISABLE
		Debug.Log(_str);
//		RecordLog.Write(_str as String);
		#endif
	}

	public static void Log(object _str,UnityEngine.Object _context){
		#if !LOG_DISABLE
		Debug.Log(_str,_context);
		#endif
	}

	public static void LogError(object _str){
		#if !LOG_DISABLE
		Debug.LogError(_str);
		#endif
	}
	
	public static void LogError(object _str,UnityEngine.Object _context){
		#if !LOG_DISABLE
		Debug.LogError(_str,_context);
		#endif
	}

	public static void LogWarning(object _str){
		#if !LOG_DISABLE
		Debug.LogWarning(_str);
		#endif
	}
	
	public static void LogWarning(object _str,UnityEngine.Object _context){
		#if !LOG_DISABLE
		Debug.LogWarning(_str,_context);
		#endif
	}

	public static void LogWarningFormat(string format, params object[] args){
		#if !LOG_DISABLE
		Debug.LogWarningFormat(format,args);
		#endif
	}

	public static void LogWarningFormat(UnityEngine.Object context, string format, params object[] args){
		#if !LOG_DISABLE
		Debug.LogWarningFormat(context,format,args);
		#endif
	}

	public static void LogException(Exception _str){
		#if !LOG_DISABLE
		Debug.LogException(_str);
		#endif
	}
	
	public static void LogException(Exception _str,UnityEngine.Object _context){
		#if !LOG_DISABLE
		Debug.LogException(_str,_context);
		#endif
	}

	public static void LogErrorFormat(string format, params object[] args){
		#if !LOG_DISABLE
		Debug.LogErrorFormat(format,args);
		#endif
	}

	public static void LogErrorFormat(UnityEngine.Object context, string format, params object[] args){
		#if !LOG_DISABLE
		Debug.LogErrorFormat(context,format,args);
		#endif
	}
}
