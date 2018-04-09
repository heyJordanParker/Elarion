using UnityEngine;

namespace Elarion.Utility {
    public class ClipboardHelper {
        private static IBoard _board;

        private static IBoard Board {
            get {
                if(_board == null) {
#if UNITY_EDITOR || UNITY_STANDALONE
                    _board = new StandaloneBoard();
#elif UNITY_ANDROID
                _board = new AndroidBoard();
                #elif UNITY_IOS
                _board = new IOSBoard ();
                #endif
                }
                return _board;
            }
        }

        public static void SetText(string str) {
            Board.SetText(str);
        }

        public static string GetText() {
            return Board.GetText();
        }
    }

    internal interface IBoard {
        void SetText(string str);
        string GetText();
    }

#if UNITY_EDITOR || UNITY_STANDALONE
    internal class StandaloneBoard : IBoard {
        public void SetText(string str) {
            GUIUtility.systemCopyBuffer = str;
        }

        public string GetText() {
            return GUIUtility.systemCopyBuffer;
        }
    }
#endif

#if UNITY_IOS
    class IOSBoard : IBoard {
        [DllImport("__Internal")]
        static extern void SetText_ (string str);
        [DllImport("__Internal")]
        static extern string GetText_();

        public void SetText(string str){
            if (Application.platform != RuntimePlatform.OSXEditor) {
                SetText_ (str);
            }
        }

        public string GetText(){
            return GetText_();
        }
    }
#endif

#if UNITY_ANDROID
internal class AndroidBoard : IBoard {
    private readonly AndroidJavaClass cb = new AndroidJavaClass("jp.ne.donuts.uniclipboard.Clipboard");

    public void SetText(string str) {
        Debug.Log("Set Text At AndroidBoard: " + str);
        cb.CallStatic("setText", str);
    }

    public string GetText() {
        return cb.CallStatic<string>("getText");
    }
}
#endif
}