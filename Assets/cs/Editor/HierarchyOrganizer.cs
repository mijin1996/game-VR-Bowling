#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class HierarchyOrganizer {
    private static readonly (string group, string[] members)[] Groups = {
        ("[Managers]",    new[] { "GameManager", "levelmanager", "buttonmanager", "SoundManager",
                                  "FloatingTextEngine", "RhythmBeatSaga", "Rhythm" }),
        ("[Spawners]",    new[] { "핀", "핀 (1)", "핀 (2)", "핀 (3)", "소켓", "소켓 (1)", "소켓 (2)" }),
        ("[Camera]",      new[] { "Main Camera", "Global Volume" }),
        ("[Environment]", new[] { "XXXXXXXBowling Club", "아래 벽이랑 다른것", "MAP", "Steps Area", "Directional Light" }),
        ("[Characters]",  new[] { "Complete XR Origin Set Up Hands Variant" }),
        ("[UI]",          new[] { "EventSystem", "CanvasStart", "CanvasScore", "CanvasMusic", "다음" }),
    };

    private static readonly string[] OldGroups = { "[Camera & Light]", "[XR]", "[Gameplay]", "[Map]" };

    [MenuItem("Tools/BowllyBoll/Hierarchy 정리")]
    public static void Organize() {
        Scene scene = SceneManager.GetActiveScene();
        HashSet<string> groupNames = new HashSet<string>(Groups.Select(g => g.group));

        Undo.IncrementCurrentGroup();
        int undoGroup = Undo.GetCurrentGroup();

        // 예전 그룹([Camera & Light], [XR], [Gameplay], [Map])은 풀어서 지운다
        foreach (GameObject old in scene.GetRootGameObjects().Where(r => OldGroups.Contains(r.name)).ToList()) {
            foreach (Transform child in old.transform.Cast<Transform>().ToList()) {
                Undo.SetTransformParent(child, null, "Organize Hierarchy");
            }
            Undo.DestroyObjectImmediate(old);
        }

        List<GameObject> roots = scene.GetRootGameObjects().ToList();
        List<string> missing = new List<string>();
        int moved = 0;

        for (int i = 0; i < Groups.Length; i++) {
            Transform group = GetOrCreateGroup(roots, Groups[i].group);
            group.SetSiblingIndex(i);

            foreach (string member in Groups[i].members) {
                List<GameObject> found = roots.Where(r => r != null && r.name.Trim() == member && !groupNames.Contains(r.name)).ToList();
                if (found.Count == 0) {
                    if (group.Cast<Transform>().All(c => c.name.Trim() != member)) missing.Add(member);
                    continue;
                }
                foreach (GameObject go in found) {
                    Undo.SetTransformParent(go.transform, group, "Organize Hierarchy");
                    go.transform.SetAsLastSibling();
                    roots.Remove(go);
                    moved++;
                }
            }
        }

        Undo.CollapseUndoOperations(undoGroup);
        EditorSceneManager.MarkSceneDirty(scene);

        List<string> leftover = roots.Where(r => r != null && !groupNames.Contains(r.name)).Select(r => r.name).ToList();
        string msg = moved + "개를 그룹 아래로 옮겼어요.\nCtrl + S 로 저장하세요. (Ctrl + Z 로 되돌리기)";
        if (missing.Count > 0) msg += "\n\n못 찾은 오브젝트: " + string.Join(", ", missing);
        if (leftover.Count > 0) msg += "\n\n목록에 없어서 루트에 남긴 것: " + string.Join(", ", leftover);
        Debug.Log("[HierarchyOrganizer] " + msg.Replace("\n", " "));
        EditorUtility.DisplayDialog("Hierarchy 정리", msg, "확인");
    }

    // 같은 이름의 그룹이 루트에 있으면 그대로 쓰고, 없으면 위치 0 / 회전 0 / 크기 1 인 빈 오브젝트를 만든다
    private static Transform GetOrCreateGroup(List<GameObject> roots, string name) {
        GameObject old = roots.FirstOrDefault(r => r != null && r.name == name);
        if (old != null) return old.transform;

        GameObject go = new GameObject(name);
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.identity;
        go.transform.localScale = Vector3.one;
        roots.Add(go);
        return go.transform;
    }
}
#endif
