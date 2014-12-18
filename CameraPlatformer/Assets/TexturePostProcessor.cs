using UnityEngine;
using UnityEditor;

public class TexturePostProcessor : AssetPostprocessor
{
	void OnPostprocessTexture(Texture2D texture)
	{
		TextureImporter importer = assetImporter as TextureImporter;
		importer.textureType  = TextureImporterType.Advanced;
		importer.isReadable = true;
		
		Object asset = AssetDatabase.LoadAssetAtPath(importer.assetPath, typeof(Texture2D));
		if (asset)
		{
			EditorUtility.SetDirty(asset);
		}
		else
		{
			importer.textureType  = TextureImporterType.Advanced;
			importer.isReadable = true;
		} 
	}
}