using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Fade : MonoBehaviour {

	public Material[] materials;
	public float waitTime = 0;
	public float fadeTime = 4;
	public bool replaceShaders = true;
	
	static Dictionary<Shader, Shader> replacementShaders = new Dictionary<Shader, Shader>();
	
	public static Shader GetReplacementFor(Shader original) {
		Shader replacement;
		if (replacementShaders.TryGetValue(original, out replacement)) return replacement;
		
		const string transparentPrefix = "Transparent/";
		const string mobilePrefix = "Mobile/";
		
		var name = original.name;
		if (name.StartsWith(mobilePrefix)) {
			name = name.Substring(mobilePrefix.Length);
		}
		if (!name.StartsWith(transparentPrefix)) {
			replacement = Shader.Find(transparentPrefix + name);
		}
		
		replacementShaders[original] = replacement;
		return replacement;
	}
	
	static string[] colorPropertyNameCandidates = new string[] { "_Color", "_TintColor" };
	
	IEnumerator Start() {
		var mat = materials;
		if (mat == null || mat.Length == 0) materials = mat = GetComponent<Renderer>().materials;
		
		if (waitTime > 0) yield return new WaitForSeconds(waitTime);
		
		if (replaceShaders) {
			foreach (var i in mat) {
				var replacement = GetReplacementFor(i.shader);
				if (replacement != null) i.shader = replacement;
			}
		}
		
		var materialCount = mat.Length;
		List<string> colorPropertyNames = new List<string>(materialCount);
		
		foreach (var m in mat) {
			var found = false;
			foreach (var candidate in colorPropertyNameCandidates) {
				found = m.HasProperty(candidate);
				if (found) {
					colorPropertyNames.Add(candidate);
					break;
				}
			}
			
			if (!found) {
				colorPropertyNames.Add(null);
			}
		}
		
		for (float t = 0; t < fadeTime; t += Time.deltaTime) {
			for (var i = 0; i < materialCount; ++i) {
				var m = mat[i];
				var colorPropertyName = colorPropertyNames[i];
				if (colorPropertyName == null) continue;
				
				var c = m.GetColor(colorPropertyName);
				c.a = 1 - (t / fadeTime);
				m.SetColor(colorPropertyName, c);
			}
			yield return null;
		}
		
		SendMessage("FadeCompleted", SendMessageOptions.DontRequireReceiver);
	}

}
