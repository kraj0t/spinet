using UnityEngine;
using System.Collections;
 
public class TextoPantalla : MonoBehaviour
{
	// Habria que mejorar esta clase para que la posicion del texto se autocalcule segun el tamaño del texto.
	// http://answers.unity3d.com/questions/13443/is-there-a-way-to-measure-the-pixel-withheight-of.html
	// http://forum.unity3d.com/threads/31351-GUIText-width-and-height
	
	
	/** SINGLETON PATTERN **/	
	private static TextoPantalla _instanceCompartida;
 
	public static TextoPantalla InstanceCompartida
	{
		get
		{
			if (_instanceCompartida == null)
				_instanceCompartida = new GameObject("TextoPantallaSingleton").AddComponent<TextoPantalla> ();
 
			return _instanceCompartida;
		}
	}
 
	public void OnApplicationQuit ()
	{
		_instanceCompartida = null;
	}	
	/** SINGLETON PATTERN END **/
	
	
	public string texto;
	
	[Range(0f, 100f)]
	public float tiempoDelay;
	public float tiempoFadeIn;
	// Usar valor negativo para no ocultarlo nunca.
	public float tiempoFull;
	// Usar valor negativo para no ocultarlo nunca.
	public float tiempoFadeOut;
	public int fontSize = 40;	
	public Color color = Color.white;
	public float leftMargin;
	public float topMargin;
	
	public bool empezarAlCrear = false;
	public bool destroyWhenDone = false;
	
	
	private const float TIEMPO_IMPOSIBLE = -99999f;
	internal float _tComienzo = TIEMPO_IMPOSIBLE;
	
	
	void Start ()
	{
		if (empezarAlCrear)
			_tComienzo = Time.time;
	}
	
	
	void OnGUI() {			
		if ( _tComienzo >= 0f )
		{
			float t = Time.time;
			if ( tiempoDelay != 0f && t < _tComienzo + Mathf.Max(0,tiempoDelay) )
				color.a = 0f;
			else if ( tiempoFadeIn != 0f && t < _tComienzo + Mathf.Max(0,tiempoDelay) + Mathf.Max(0,tiempoFadeIn) )
				color.a = (t - _tComienzo - tiempoDelay) / tiempoFadeIn;
			else
			{
				if ( tiempoFull < 0f || tiempoFadeOut < 0f || tiempoFull != 0f && t < _tComienzo + Mathf.Max(0,tiempoDelay) + Mathf.Max(0,tiempoFadeIn) + Mathf.Max(0,tiempoFull) )
					color.a = 1f;
				else if ( tiempoFadeOut != 0f && t < _tComienzo + Mathf.Max(0,tiempoDelay) + Mathf.Max(0,tiempoFadeIn) + Mathf.Max(0,tiempoFull) + Mathf.Max(0,tiempoFadeOut) )
					color.a = 1f - ( (t - _tComienzo - tiempoFadeIn - tiempoFull) / tiempoFadeOut );
				else
					_tComienzo = TIEMPO_IMPOSIBLE;
			}					
			
			GUI.depth = -10000;
			GUIStyle style = GUI.skin.label;
		    style.fontSize = Screen.width / fontSize;
			style.normal.textColor = color;
		    GUI.Label( new Rect(Screen.width * leftMargin, Screen.height * topMargin, Screen.width * (1-leftMargin*2), Screen.height * (1-topMargin) ), texto, style);
		}
    }
	
	public void MostrarAhora ()
	{
		_tComienzo = Time.time;
	}
	
	public bool IsMostrando ()
	{
		return ( _tComienzo != TIEMPO_IMPOSIBLE );
	}
	
	public void Ocultar ()
	{
		_tComienzo = TIEMPO_IMPOSIBLE;
	}
	
	// Devuelve el tiempo actual en segundos del mensaje.
	// Devuelve -1 si no se esta mostrando (usar isMostrando para eso).
	public float GetTiempoActual ()
	{
		if ( _tComienzo < 0 )
			return -1f;
		return Time.time - _tComienzo;
	}
	
	public void SetTiempoActual ( float segundos )
	{
		_tComienzo = Time.time - segundos;
	}
	
	public void FadeOutAhora ( float tiempoFadeOut )
	{
		if ( tiempoFull < 0f )
			tiempoFull = 0f;
		this.tiempoFadeOut = tiempoFadeOut;
		_tComienzo = Time.time - Mathf.Max(0,tiempoDelay) - Mathf.Max(0,tiempoFadeIn) - Mathf.Max(0,tiempoFull);
	}
	
	public float GetDuracion ()
	{
		return Mathf.Max(0,tiempoDelay) + Mathf.Max(0,tiempoFadeIn) + Mathf.Max(0,tiempoFull) + Mathf.Max(0,tiempoFadeOut);
	}
	
	public void mostrarTexto ( string texto, float tiempoDelay, float tiempoFadeIn, float tiempoFull, float tiempoFadeOut, Color color, int fontSize = 40, float leftMargin = 0.2f, float topMargin = 0.8f )
	{
		this.texto = texto;
		this.tiempoDelay = tiempoDelay;
		this.tiempoFadeIn = tiempoFadeIn;
		this.tiempoFull = tiempoFull;
		this.tiempoFadeOut = tiempoFadeOut;
		this.fontSize = fontSize;
		this.color = color;
		this.leftMargin = leftMargin;
		this.topMargin = topMargin;
		
		destroyWhenDone = false;
		_tComienzo = Time.time;
	}
	
	public static void mostrarTextoUnaVez ( string texto, float tiempoDelay, float tiempoFadeIn, float tiempoFull, float tiempoFadeOut, Color color, int fontSize = 40, float leftMargin = 0.2f, float topMargin = 0.8f )
	{
		TextoPantalla i = new GameObject("TextoPantalla(mostrar_una_vez)").AddComponent<TextoPantalla> ();
		i.mostrarTexto( texto, tiempoDelay, tiempoFadeIn, tiempoFadeOut, tiempoFull, color, fontSize, leftMargin, topMargin );
	}
	
}



