using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using System.Text;
using System.IO;

public class AssetImporterWindow : MonoBehaviour
{
	Texture2D pic;
	Color[] pix;
	public GameObject cube;
	GameObject go;
	int pixelSkip;
	float cubeSize;
	public GameObject player;
	float playerSize;
	public float f;

    [MenuItem("Window/Asset Importer")]
    static void OpenWindow()
    {
        //var window = ScriptableObject.CreateInstance<AssetImporterWindow>();
        //window.title = "Asset Importer";
        //window.Show();
    }
    //public Object destinationFolder;
    //public Object templateAsset;
    public string assetsPaths = "";
	public string str1 = "";
	public string str2 = "";
	public string str3 = "";
    private string newAssetPath = null;
    Vector2 scrollPosition = new Vector2();

    void OnGUI()
    {
        if (Event.current.type == EventType.Layout)
        {
            if (newAssetPath != null)
            {
                GUIUtility.keyboardControl = 0;
                GUIUtility.hotControl = 0;
                assetsPaths = newAssetPath;
                newAssetPath = null;
            }
            if (assetsPaths.IndexOf('\\') >= 0)
            {
                assetsPaths = assetsPaths.Replace('\\', '/');
            }
            //if (destinationFolder != null)
           // {
                string destpath = Application.dataPath + "/Resources";
               // if (string.IsNullOrEmpty(destpath))
               // {
                    //destinationFolder = null;
              //  }
              //  else if (!System.IO.Directory.Exists(destpath))
              //  {
                //    destpath = destpath.Substring(0, destpath.LastIndexOf('/'));
               //     destinationFolder = AssetDatabase.LoadMainAssetAtPath(destpath);
               // }
          //  }
        }

        GUILayout.BeginHorizontal("Toolbar"); GUILayout.Label("");  GUILayout.EndHorizontal();

       // templateAsset = EditorGUILayout.ObjectField("Template Asset", templateAsset, typeof(Object), false);
       // destinationFolder = EditorGUILayout.ObjectField("Destination Folder", destinationFolder, typeof(Object), false);

        GUILayout.Label("Where to find picture on your computer");
        GUILayout.Space(2);
        scrollPosition = GUILayout.BeginScrollView(scrollPosition);

        assetsPaths = GUILayout.TextArea(assetsPaths);
		GUILayout.Label("Pixel skip (positive integer -- 1-4 can lag alot");
		str1 = GUILayout.TextArea(str1);
		int.TryParse(str1, out pixelSkip);
		GUILayout.Label("Block size (positive float)");
		str2 = GUILayout.TextArea(str2);
		float.TryParse(str2, out cubeSize);
		GUILayout.Label("Player size (positive float >= 0.5)");
		str3 = GUILayout.TextArea(str3);
		float.TryParse(str3, out playerSize);
        Event evt = Event.current;
        Rect drop_area = GUILayoutUtility.GetLastRect();
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    StringBuilder sb = new StringBuilder();
                    foreach (var path in assetsPaths.Split('\n', '\r'))
                    {
                        if (string.IsNullOrEmpty(path)) continue;
                        sb.AppendFormat("{0}\n", path.ToString());
                    }
                    foreach (var path in DragAndDrop.paths)
                    {
                        if (string.IsNullOrEmpty(path)) continue;
                        sb.AppendFormat("{0}\n", path.ToString());
                    }
                    newAssetPath = sb.ToString();
                }
                break;
        }
        GUILayout.EndScrollView();

        if (GUILayout.Button("Import"))
        {
            var start = System.DateTime.Now;
            string destpath;
           // if( destinationFolder != null )
           // {
                destpath = Application.dataPath + "/Resources";
          //  }
           // else
           // {
          //      destpath = AssetDatabase.GetAssetPath(templateAsset);
         //       destpath = destpath.Substring(0,destpath.LastIndexOf('/'));
         //   }

            List<Object> assets = new List<Object>();
			string assetDest = "";
            foreach (var assetPath in assetsPaths.Split('\n', '\r'))
            {
                if (string.IsNullOrEmpty(assetPath)) continue;

                assetDest = destpath + assetPath.Substring(assetPath.LastIndexOf('/'));
                //assetDest = AssetDatabase.GenerateUniqueAssetPath(assetDest);
                AssetDatabase.CopyAsset(assetPath, destpath);
                System.IO.File.Copy(assetPath, assetDest, true);
                //AssetDatabase.ImportAsset(assetDest);
                assets.Add(AssetDatabase.LoadAssetAtPath(assetDest, typeof(Texture2D)));
            }
            AssetDatabase.SaveAssets();
           
            Selection.instanceIDs = new int[0];
            Selection.objects = assets.ToArray();

			AssetDatabase.Refresh();
			string str = assetDest.Substring(assetDest.LastIndexOf('/') + 1);
			pic = (Texture2D) Resources.Load(str.Replace(".jpeg", "").Replace(".jpg", "").Replace(".png", "")) as Texture2D;
			BuildLevel ();
        }
        GUILayout.Space(6);
		if (GUILayout.Button("Reset"))
			Application.LoadLevel(0);
    }

	void BuildLevel ()
	{
		pix = pic.GetPixels();
		for (int x = 0; x < pic.width - pixelSkip; x += pixelSkip)
		{
			for (int y = 0; y < pic.height - pixelSkip; y += pixelSkip)
			{
				go = (GameObject) GameObject.Instantiate(cube, new Vector2(x / pixelSkip * cubeSize, y / pixelSkip * cubeSize), Quaternion.identity);
				go.renderer.material.color = pic.GetPixel(x, y);
				go.transform.localScale = Vector3.one * cubeSize;
				if (go.renderer.material.color.r > go.renderer.material.color.g && go.renderer.material.color.r > go.renderer.material.color.b && (go.renderer.material.color.r - go.renderer.material.color.g) + (go.renderer.material.color.r - go.renderer.material.color.b) > f)
				{
					go.AddComponent<Hazard>();
					go.collider.isTrigger = true;
				}
				else if (go.renderer.material.color.b > go.renderer.material.color.r && go.renderer.material.color.b > go.renderer.material.color.g && (go.renderer.material.color.b - go.renderer.material.color.r) + (go.renderer.material.color.b - go.renderer.material.color.g) > f)
				{
					Destroy(go);
					if (GameObject.Find ("Player(Clone)") == null)
					{
						go = (GameObject) GameObject.Instantiate(player, new Vector2(x / pixelSkip * cubeSize, y / pixelSkip * cubeSize), Quaternion.identity);
						go.transform.localScale = Vector3.one * playerSize;
						Camera.main.orthographicSize *= playerSize;
					}
				}
				//else if (go.renderer.material.color.r < .6f && go.renderer.material.color.g < .6f && go.renderer.material.color.b < .6f && go.renderer.material.color.r > .4f && go.renderer.material.color.g > .4f && go.renderer.material.color.b > .4f)
				//{
				//	go.name = go.name + "(Wall Jump)";
				//}
				//else if (!(go.renderer.material.color.r < .4f && go.renderer.material.color.g < .4f && go.renderer.material.color.b < .4f) && !(go.renderer.material.color.g > go.renderer.material.color.r && go.renderer.material.color.g > go.renderer.material.color.b && (go.renderer.material.color.g - go.renderer.material.color.r) + (go.renderer.material.color.g - go.renderer.material.color.b) > f))
				//	Destroy(go);
				else if (!(go.renderer.material.color.r < .5f && go.renderer.material.color.g < .5f && go.renderer.material.color.b < .5f) && !(go.renderer.material.color.g > go.renderer.material.color.r && go.renderer.material.color.g > go.renderer.material.color.b && (go.renderer.material.color.g - go.renderer.material.color.r) + (go.renderer.material.color.g - go.renderer.material.color.b) > f))
					Destroy(go);
			}
		}
	}
}
