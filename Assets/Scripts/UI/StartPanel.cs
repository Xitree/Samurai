using UnityEngine;

namespace UI{
    public class StartPanel : BasePanel
    {
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
        
        }

        public override void ShowMe() {
        
        }

        public override void HideMe() {
            
        }

        protected override void ClickBtn(string btnName) {
            switch (btnName) {
                case "StartBtn":
                    UIMgr.Instance.HidePanel<StartPanel>();
                    break;
                case "ExitBtn":
                    //退出游戏
                    Application.Quit();
                    break;
                case "SetBtn":
                    print(btnName);
                    break;
            }
        }
    }
}
