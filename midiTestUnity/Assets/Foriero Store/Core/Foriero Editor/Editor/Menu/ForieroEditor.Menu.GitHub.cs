using UnityEngine;
using UnityEditor;
using System.Collections;
using System.IO;
using ForieroEditor.Extensions;

namespace ForieroEditor
{
	public static partial class Menu
	{
		[MenuItem ("Foriero/GitHub/Get Spine Runtime")]
		public static void GetSpineRuntime ()
		{
			string spine_unity = "https://github.com/EsotericSoftware/spine-runtimes/trunk/spine-unity/Assets/spine-unity/";
			string spine_src = "https://github.com/EsotericSoftware/spine-runtimes/trunk/spine-csharp/src/";


			GitHub.GetRepositoryFiles (spine_unity, "Assets/Spine/spine-unity");
			GitHub.GetRepositoryFiles (spine_src, "Assets/Spine/src");

			AssetDatabase.Refresh ();
		}

		[MenuItem ("Foriero/GitHub/Get DOTween")]
		public static void GetDOTween ()
		{
			string dotween = "https://github.com/Demigiant/dotween/trunk/_DOTween.Assembly/bin";
			GitHub.GetRepositoryFiles (dotween, "Assets/DOTween");
			AssetDatabase.Refresh ();
		}

		[MenuItem ("Foriero/GitHub/Get FontAwesome")]
		public static void GetFontAwesome ()
		{
			string font = "https://github.com/FortAwesome/Font-Awesome/trunk/fonts/fontawesome-webfont.ttf";
			string variables = "https://github.com/FortAwesome/Font-Awesome/trunk/less/variables.less";

			string[] files = Directory.GetFiles (Directory.GetCurrentDirectory (), "FontAwesome.ttf", SearchOption.AllDirectories);

			string path = "Assets/Fonts";

			if (files.Length != 0) {
				path = Path.GetDirectoryName (files [0]).RemoveProjectPath ();
			}

			path = path.FixAssetsPath ();

			GitHub.GetRepositoryFiles (font, path + "/FontAwesome.ttf");
			GitHub.GetRepositoryFiles (variables, path + "/variables.less");

			AssetDatabase.Refresh ();
		}
	}
}