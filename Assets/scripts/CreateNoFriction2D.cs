#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

public class CreateNoFriction2D
{
    [MenuItem("Tools/Create NoFriction 2D Material")]
    public static void CreateMaterial()
    {
        var mat = new PhysicsMaterial2D("NoFriction2D");
        mat.friction = 0f;
        mat.bounciness = 0f;

        AssetDatabase.CreateAsset(mat, "Assets/NoFriction2D.physicsMaterial2D");
        AssetDatabase.SaveAssets();

        EditorUtility.FocusProjectWindow();
        Selection.activeObject = mat;

        Debug.Log("✅ NoFriction2D 已建立成功！");
    }
}
#endif
