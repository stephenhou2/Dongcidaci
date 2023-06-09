﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

namespace GameEngine
{
    public class GameSceneManager : ModuleManager<GameSceneManager>
    {
        private Dictionary<string, IScene> mSceneMap = new Dictionary<string, IScene>();
        private IScene mCurScene;

        public override void Initialize()
        {
            foreach(KeyValuePair<string,IScene> kv in SceneDefine.RegisteredScenes)
            {
                mSceneMap.Add(kv.Key, kv.Value);
            }
        }

        public override void Dispose()
        {
            mSceneMap = null;
            mCurScene = null;
        }

        public void SwitchToScene(string sceneName)
        {
            if(string.IsNullOrEmpty(sceneName))
            {
                Log.Error(LogLevel.Critical, "SwitchToScene Failed, Empty scene name is not valid!");
                return;
            }

            IScene toScene = SceneDefine.GetScene(sceneName);
            if(toScene == null)
            {
                Log.Error(LogLevel.Critical, "SwitchToScene Failed, scene [{0}] not registerd! ", sceneName);
                return;
            }

            if (mCurScene != null)
            {
                // 要切换到同一个scene
                if (mCurScene.GetSceneName().Equals(sceneName))
                    return;

                Log.Logic(LogLevel.Info, "Exit {0} Scene...", mCurScene.GetSceneName());
                mCurScene.OnSceneExit();
                mCurScene.AfterSceneExit();
                InputManager.Ins.AddState(InputStateDefine.EMPTY_STATE);
            }

            mCurScene = toScene;
            InputManager.Ins.RemoveState(InputStateDefine.EMPTY_STATE);
            Log.Logic(LogLevel.Info, "Enter {0} Scene...", toScene.GetSceneName());
            mCurScene.OnSceneEnter();
            mCurScene.BeforeSceneEnter();
        }

        public void LoadAndSwitchToScene(string sceneName)
        {
            // 要切换的scene和当前所处的scene相同时，不执行切换操作
            if (mCurScene != null && mCurScene.GetSceneName().Equals(sceneName))
            {
                Log.Error(LogLevel.Info, "already in scene [{0}]", sceneName);
                return;
            }

            IEnumerator routine = AsyncLoadScene(sceneName);
            GameManager.Ins.CoroutineMgr.StartCoroutine(routine);
        }

        private IEnumerator AsyncLoadScene(string sceneName)
        {
            if (!mSceneMap.ContainsKey(sceneName))
            {
                Log.Error(LogLevel.Fatal, "AsyncLoadScene {0} Failed,not defined!", sceneName);
                yield break;
            }

            var op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone)
            {
                Log.Logic(LogLevel.Info, "Async loading scene {0} ...... progress:{1}", sceneName, op.progress);
                yield return null;
            }

            SwitchToScene(sceneName);
        }

        public override void OnUpdate(float deltaTime)
        {
            if (mCurScene != null)
            {
                mCurScene.OnSceneUpdate(deltaTime);
            }
        }

        public override void OnLateUpdate(float deltaTime)
        {
            if (mCurScene != null)
            {
                mCurScene.OnSceneLateUpdate(deltaTime);
            }
        }
    }
}
