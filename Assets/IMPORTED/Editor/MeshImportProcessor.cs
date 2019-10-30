using UnityEngine;
using UnityEditor;

class MeshImportProcessor : AssetPostprocessor
{
    void OnPreprocessModel()
    {
        (assetImporter as ModelImporter).globalScale = 1.0f;
        (assetImporter as ModelImporter).materialName = ModelImporterMaterialName.BasedOnMaterialName;
        (assetImporter as ModelImporter).materialSearch = ModelImporterMaterialSearch.Everywhere;
    }

    Material OnAssignMaterialModel ( Material material, Renderer renderer ) 
    {
        Material returnedMaterial = null;

		string materialPath = "Assets/" + material.name + ".mat";
			
        // Find if there is a material at the material path
		// Turn this off to always regeneration materials
        Material existingMaterial = (Material)AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material));
        if (existingMaterial)
        {
            returnedMaterial = existingMaterial;
        }
        else
        {
			/*
            // Create a new material asset using the specular shader
            // but otherwise the default values from the model
            material.shader = Shader.Find("Specular");
            AssetDatabase.CreateAsset(material, "Assets/" + material.name + ".mat");
            returnedMaterial = material;
			*/
        }

        return returnedMaterial;
	}

}