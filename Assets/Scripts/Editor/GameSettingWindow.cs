using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TetrisEngine;
using TetrisEngine.BlockPiece;
using UnityEditor;
using UnityEngine;

public class GameSettingWindow : EditorWindow
{
    private GUISkin mGuiSkin;
    private GUIStyle mSelectedStyle;
    private GUIStyle mNormalStyle;
    private GameSettings mGameSettings;
    private int[][][] mBlockLayout;
    private Vector2Int[] mInitialPosition = new Vector2Int[Block.BLOCK_ROTATIONS];
    private Color mColor;
    private string mName;
    private float mTimeToStep;
    private int mPointsByBreakingLine;
    private bool mControledRandomMode;
    private bool mDebugMode;
    private KeyCode mMoveRightKey;
    private KeyCode mMoveLeftKey;
    private KeyCode mMoveDownKey;
    private KeyCode mRotateRightKey;
    private KeyCode mRotateLeftKey;
    private Vector2 mScrollPosition;

    private Vector2 mScrollImportedPosition;
    private int mCurrentEditing = -1;

    /// <summary>
    /// Create a custom setting on Editor Window
    /// </summary>
    [MenuItem("Window/GameSettings Creator")]
    static void Init()
    {
        GameSettingWindow window = (GameSettingWindow)GetWindow(typeof(GameSettingWindow));
        // Open the window
        window.Show();
    }

    private void OnEnable()
    {
        mCurrentEditing = -1;
        mBlockLayout = GetEmptyLayout();

        // Load file .guiskin
        mGuiSkin = Resources.Load<GUISkin>("GameSettingsCreatorSkin");
        // Then load GUIStyle from GUISkin
        mSelectedStyle = mGuiSkin.GetStyle("Selected");
        mNormalStyle = mGuiSkin.GetStyle("Normal");
    }

    void OnGUI()
    {
        GUI.skin = mGuiSkin;
        // Display GUI of custom window
        mScrollPosition = GUILayout.BeginScrollView(mScrollPosition, true, true, GUILayout.Width(position.width), GUILayout.Height(position.height));

        // Create a horizontal layout with 2 buttons
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();

        if (GUILayout.Button("IMPORT JSON", GUILayout.Width(150), GUILayout.Height(50)))
        {
            ImportJson();
        }

        if (GUILayout.Button("CREATE NEW", GUILayout.Width(150), GUILayout.Height(50)))
        {
            mCurrentEditing = -1;
            mGameSettings = new GameSettings
            {
                pieces = new List<BlockSpec>()
            };
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        if (mGameSettings == null)
        {
            GUILayout.EndScrollView();
            return;
        }

        mTimeToStep = EditorGUILayout.FloatField("Time between update step", mTimeToStep);
        if (mTimeToStep < 0.01f)
        {
            mTimeToStep = 0.01f;
        }

        mPointsByBreakingLine = EditorGUILayout.IntField("Points by breaking lines", mPointsByBreakingLine);
        if (mPointsByBreakingLine < 0)
        {
            mPointsByBreakingLine = 0;
        }

        mControledRandomMode = EditorGUILayout.ToggleLeft("Controled random mode", mControledRandomMode);
        mDebugMode = EditorGUILayout.ToggleLeft("Debug mode", mDebugMode);
        mMoveRightKey = (KeyCode)EditorGUILayout.EnumPopup("Key to move the right", mMoveRightKey);
        mMoveLeftKey = (KeyCode)EditorGUILayout.EnumPopup("Key to move the left", mMoveLeftKey);
        mMoveDownKey = (KeyCode)EditorGUILayout.EnumPopup("Key to make the piece fall faster", mMoveDownKey);
        mRotateRightKey = (KeyCode)EditorGUILayout.EnumPopup("Key to rotate right", mRotateRightKey);
        mRotateLeftKey = (KeyCode)EditorGUILayout.EnumPopup("Key to rotate left", mRotateLeftKey);

        GUILayout.Space(20);
        // Scroll view to show list of all blocks with name
        var pieceHeight = 30f;
        var scrollHeight = Mathf.Clamp(pieceHeight * (2 + mGameSettings.pieces.Count), 0, pieceHeight * 4);
        mScrollImportedPosition = GUILayout.BeginScrollView(
            mScrollImportedPosition, true, true,
            GUILayout.Width(position.width - 30),
            GUILayout.Height(scrollHeight));

        // Create a new block shape
        if (GUILayout.Button("ADD NEW", GUILayout.Width(position.width - 60), GUILayout.Height(pieceHeight)))
        {
            mScrollImportedPosition = new Vector2(0, scrollHeight);
            BeginEdit(-1, new BlockSpec());
        }

        bool? mFinishedLayout = null;
        int counter = 0;
        for (int i = 0; i < mGameSettings.pieces.Count; i++)
        {
            if (counter++ == 0)
            {
                GUILayout.BeginHorizontal();
                mFinishedLayout = false;
            }

            var index = i;
            GUI.color = mGameSettings.pieces[i].color;
            // Click on a specific block to edit
            if (GUILayout.Button(mGameSettings.pieces[i].name, GUILayout.Width(150), GUILayout.Height(pieceHeight)))
            {
                BeginEdit(index, mGameSettings.pieces[i]);
            }

            // Each line has 4 blocks
            if (counter == 4)
            {
                GUILayout.EndHorizontal();
                mFinishedLayout = true;
                counter = 0;
            }
        }

        GUI.color = Color.white;
        if (mFinishedLayout.HasValue && !mFinishedLayout.Value)
        {
            GUILayout.EndHorizontal();
        }
        GUILayout.EndScrollView();

        if (mCurrentEditing != -1)
        {
            GUILayout.Space(30);
            mName = EditorGUILayout.TextField("Block name", mName);
            GUILayout.Space(10);
            mColor = EditorGUILayout.ColorField("Block Color", mColor);
            mColor.a = 1f;
            GUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            for (int t = 0; t < Block.BLOCK_ROTATIONS; t++)
            {
                GUILayout.TextArea("Rotation " + (t + 1));
            }
            EditorGUILayout.EndHorizontal();

            for (int i = 0; i < Block.BLOCK_AREA; i++)
            {
                EditorGUILayout.BeginHorizontal();
                for (int j = 0; j < Block.BLOCK_ROTATIONS; j++)
                {
                    EditorGUILayout.BeginHorizontal();
                    for (int l = 0; l < Block.BLOCK_AREA; l++)
                    {
                        bool active = mBlockLayout[j][i][l] == 1;
                        GUI.color = active ? mColor : Color.white;
                        if (GUILayout.Button(string.Format("{0},{1}", i, l), active ? mSelectedStyle : mNormalStyle))
                            mBlockLayout[j][i][l] = active ? 0 : 1;
                        GUI.color = Color.white;
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            for (int i = 0; i < mInitialPosition.Length; i++)
            {
                mInitialPosition[i] = EditorGUILayout.Vector2IntField("Init Pos", mInitialPosition[i], GUILayout.Width(position.width / Block.BLOCK_ROTATIONS - 10));
            }
            EditorGUILayout.EndHorizontal();
            EndEdit();

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUI.color = Color.red;
            if (GUILayout.Button("REMOVE", GUILayout.Width(150), GUILayout.Height(50)))
            {
                RemoveSelected();
            }
            GUI.color = Color.white;

            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }

        GUILayout.Space(50);
        GUILayout.BeginHorizontal();
        GUILayout.FlexibleSpace();
        if (GUILayout.Button("SAVE", GUILayout.Width(150), GUILayout.Height(50)))
        {
            ExportSettings();
        }

        GUILayout.FlexibleSpace();
        GUILayout.EndHorizontal();

        GUILayout.EndScrollView();
    }

    private int[][][] GetEmptyLayout()
    {
        var layout = new int[Block.BLOCK_ROTATIONS][][];
        for (int i = 0; i < layout.Length; i++)
        {
            layout[i] = new int[Block.BLOCK_AREA][];
            for (int j = 0; j < layout[i].Length; j++)
            {
                layout[i][j] = new int[Block.BLOCK_AREA];
            }
        }

        return layout;
    }

    private void ImportJson()
    {
        // Open file dialog to choose json file
        var path = EditorUtility.OpenFilePanel("Choose GameSettings json.", "Assets", "json");
        if (path.Length != 0)
        {
            var json = File.ReadAllText(path);
            // Parse json to GameSettings object
            mGameSettings = JsonUtility.FromJson<GameSettings>(json);
            mTimeToStep = mGameSettings.TimeToStep;
            mPointsByBreakingLine = mGameSettings.PointsByBreakingLine;
            mControledRandomMode = mGameSettings.ControledRandomMode;
            mDebugMode = mGameSettings.DebugMode;
            mMoveRightKey = mGameSettings.MoveRightKey;
            mMoveLeftKey = mGameSettings.MoveLeftKey;
            mMoveDownKey = mGameSettings.MoveDownKey;
            mRotateLeftKey = mGameSettings.RotateLeftKey;
            mRotateRightKey = mGameSettings.RotateRightKey;

            mCurrentEditing = -1;
        }
    }

    private void BeginEdit(int index, BlockSpec specs)
    {
        if (index == -1)
        {
            specs.name = "New Block";
            specs.color = Color.white;
            // Init blocks with all are empty cells
            specs.serializedBlockPositions = GetSerializableLayout(GetEmptyLayout());
            specs.initialPosition = GetInitialPositions();
            mGameSettings.pieces.Add(specs);
            index = mGameSettings.pieces.Count - 1;
        }

        GUIUtility.keyboardControl = 0;
        GUIUtility.hotControl = 0;
        mCurrentEditing = index;

        var pos = 0;
        var blockPositions = new int[Block.BLOCK_ROTATIONS][][];
        for (int i = 0; i < blockPositions.Length; i++)
        {
            blockPositions[i] = new int[Block.BLOCK_AREA][];
            for (int j = 0; j < blockPositions[i].Length; j++)
            {
                blockPositions[i][j] = new int[Block.BLOCK_AREA];
                for (int k = 0; k < blockPositions[i][j].Length; k++)
                {
                    blockPositions[i][j][k] = specs.serializedBlockPositions[pos++];
                }
            }
        }

        mBlockLayout = blockPositions;
        mInitialPosition = specs.initialPosition;
        mColor = specs.color;
        mName = specs.name;
    }

    private void EndEdit()
    {
        if (mGameSettings != null)
        {
            var specs = new BlockSpec
            {
                name = mName,
                color = mColor,
                initialPosition = mInitialPosition,
                serializedBlockPositions = GetSerializableLayout(mBlockLayout)
            };
            mGameSettings.pieces[mCurrentEditing] = specs;
        }
    }

    private List<int> GetSerializableLayout(int[][][] layout)
    {
        var serializableLayout = new List<int>();
        for (int i = 0; i < Block.BLOCK_ROTATIONS; i++)
        {
            for (int j = 0; j < Block.BLOCK_AREA; j++)
            {
                for (int l = 0; l < Block.BLOCK_AREA; l++)
                {
                    serializableLayout.Add(layout[i][j][l]);
                }
            }
        }

        return serializableLayout;
    }

    private Vector2Int[] GetInitialPositions()
    {
        var initialPositions = new Vector2Int[Block.BLOCK_ROTATIONS];
        for (int i = 0; i < Block.BLOCK_ROTATIONS; i++)
        {
            initialPositions[i] = new Vector2Int(-Block.BLOCK_AREA / 2, -Block.BLOCK_AREA / 2);
        }
        return initialPositions;
    }

    private void RemoveSelected()
    {
        mGameSettings.pieces.RemoveAt(mCurrentEditing);
        mCurrentEditing = -1;
    }

    private void ExportSettings()
    {
        var elements = 0;
        var squaredArea = Block.BLOCK_AREA * Block.BLOCK_AREA;
        foreach (var piece in mGameSettings.pieces)
        {
            elements = 0;
            while (elements * squaredArea < piece.serializedBlockPositions.Count)
            {
                if (piece.serializedBlockPositions.Skip(elements * squaredArea).Take(squaredArea).Sum() == 0)
                {
                    throw new System.Exception(string.Format("Exportation failed. Rotation number {0} of piece {1} was left empty. A tetrimino must have at least one block.", elements + 1, piece.name));
                }
                elements++;
            }
        }
        mGameSettings.TimeToStep = mTimeToStep;
        mGameSettings.PointsByBreakingLine = mPointsByBreakingLine;
        mGameSettings.ControledRandomMode = mControledRandomMode;
        mGameSettings.DebugMode = mDebugMode;

        CheckValidKey(mMoveRightKey, "Move Right");
        mGameSettings.MoveRightKey = mMoveRightKey;
        CheckValidKey(mMoveLeftKey, "Move Left");
        mGameSettings.MoveLeftKey = mMoveLeftKey;
        CheckValidKey(mMoveDownKey, "Move Down");
        mGameSettings.MoveDownKey = mMoveDownKey;
        CheckValidKey(mRotateRightKey, "Rotate Right");
        mGameSettings.RotateRightKey = mRotateRightKey;
        CheckValidKey(mRotateLeftKey, "Rotate Left");
        mGameSettings.RotateLeftKey = mRotateLeftKey;

        var json = JsonUtility.ToJson(mGameSettings, true);
        string path = EditorUtility.SaveFilePanel("Save GameSettings Json", "Assets", "GameSettings", "json");
        if (path.Length == 0)
        {
            Debug.LogError("Invalid path.");
            return;
        }
        File.WriteAllText(path, json);
    }

    private void CheckValidKey(KeyCode code, string keyTitle)
    {
        if (code == KeyCode.None)
        {
            throw new System.Exception(string.Format("Exportation failed. {0} key cannot be None", keyTitle));
        }

    }
}
