using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public static class SetupProject
{
    [MenuItem("Tools/Setup Project - MageShop & Souls")]
    public static void RunSetup()
    {
        // 1. Setup Castle scene: Mage GameObject
        SetupCastleScene();

        // 2. Setup Enemy prefabs: assign SoulPrefab
        SetupEnemyPrefabs();

        // 3. Verify Soul prefab
        VerifySoulPrefab();

        Debug.Log("Setup completo. Revisa los mensajes anteriores.");
    }

    static void SetupCastleScene()
    {
        string scenePath = "Assets/Scenes/Castle_MainScene.unity";
        if (!File.Exists(scenePath))
        {
            Debug.LogError("No se encuentra Castle_MainScene.unity en " + scenePath);
            return;
        }

        EditorSceneManager.OpenScene(scenePath);

        // Find Mage GameObject
        GameObject mage = GameObject.Find("Mage");
        if (mage == null)
        {
            Debug.LogError("No se encuentra GameObject 'Mage' en Castle_MainScene.");
            return;
        }

        // Remove NPC.cs if present
        MonoBehaviour npc = mage.GetComponent<NPC>() as MonoBehaviour;
        if (npc != null)
        {
            Object.DestroyImmediate(npc);
            Debug.Log("NPC.cs eliminado de Mage.");
        }

        // Add MageShop.cs if not already present
        MageShop shop = mage.GetComponent<MageShop>();
        if (shop == null)
        {
            shop = mage.AddComponent<MageShop>();
            Debug.Log("MageShop.cs añadido a Mage.");
        }

        // Configure MageShop fields
        shop.interactionRange = 2f;

        // Try to find the interact prompt (child named "InteractPrompt" or similar)
        Transform prompt = mage.transform.Find("InteractPrompt");
        if (prompt != null)
            shop.interactPrompt = prompt.gameObject;
        else
            Debug.LogWarning("Mage no tiene un hijo 'InteractPrompt'. Crea uno con un TMP_Text o Image.");

        // Set dialogue lines (narrative after first shop close)
        shop.dialogueLines = new string[]
        {
            "Así que has descubierto la tienda...",
            "Vuelve cuando tengas más almas, las mejores aguardan.",
            "Cada alma cuenta una historia. Las tuyas empiezan a ser interesantes."
        };

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());
        Debug.Log("Castle_MainScene guardada.");
    }

    static void SetupEnemyPrefabs()
    {
        string[] enemyPrefabs = new string[]
        {
            "Assets/Prefabs/GoblinGreen.prefab",
            "Assets/Prefabs/GoblinRed.prefab",
            "Assets/Prefabs/GoblinBoss.prefab"
        };

        string soulPrefabPath = "Assets/Prefabs/SoulPrefab.prefab";
        GameObject soulPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(soulPrefabPath);
        if (soulPrefab == null)
        {
            Debug.LogError("No se encuentra SoulPrefab.prefab en " + soulPrefabPath);
            return;
        }

        foreach (string path in enemyPrefabs)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning("No se encuentra prefab: " + path);
                continue;
            }

            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            if (prefab == null)
            {
                Debug.LogWarning("No se pudo cargar prefab: " + path);
                continue;
            }

            Enemy enemy = prefab.GetComponent<Enemy>();
            if (enemy == null)
            {
                Debug.LogWarning(prefab.name + " no tiene componente Enemy.");
                continue;
            }

            SerializedObject so = new SerializedObject(enemy);
            SerializedProperty prop = so.FindProperty("SoulPrefab");
            if (prop != null)
            {
                prop.objectReferenceValue = soulPrefab;
                so.ApplyModifiedProperties();
                Debug.Log(prefab.name + ": SoulPrefab asignado.");
            }
            else
            {
                Debug.LogError(prefab.name + ": No se encuentra campo 'SoulPrefab' en Enemy. Recompila los scripts.");
            }

            // Set soulDropAmount per enemy type
            SerializedProperty dropProp = so.FindProperty("soulDropAmount");
            if (dropProp != null)
            {
                if (prefab.name.Contains("Boss"))
                    dropProp.intValue = 5;
                else if (prefab.name.Contains("Red"))
                    dropProp.intValue = 2;
                else
                    dropProp.intValue = 1;
                so.ApplyModifiedProperties();
                Debug.Log(prefab.name + ": soulDropAmount = " + dropProp.intValue);
            }

            EditorUtility.SetDirty(prefab);
            PrefabUtility.SavePrefabAsset(prefab);
        }

        AssetDatabase.SaveAssets();
        Debug.Log("Prefabs de enemigos actualizados.");
    }

    static void VerifySoulPrefab()
    {
        string path = "Assets/Prefabs/SoulPrefab.prefab";
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null)
        {
            Debug.LogError("SoulPrefab.prefab no encontrado.");
            return;
        }

        Collider2D col = prefab.GetComponent<Collider2D>();
        if (col == null)
        {
            Debug.LogError("SoulPrefab no tiene Collider2D. Agrégale un CircleCollider2D como Trigger.");
        }
        else if (!col.isTrigger)
        {
            col.isTrigger = true;
            EditorUtility.SetDirty(prefab);
            PrefabUtility.SavePrefabAsset(prefab);
            Debug.Log("SoulPrefab: Collider2D marcado como Trigger.");
        }
        else
        {
            Debug.Log("SoulPrefab: Collider2D Trigger OK.");
        }

        if (prefab.GetComponent<Rigidbody2D>() == null)
        {
            Debug.LogWarning("SoulPrefab no tiene Rigidbody2D. Las colisiones Trigger 2D funcionan sin él, pero añádelo si hay problemas.");
        }

        Debug.Log("Verificación de SoulPrefab completada.");
    }
}
