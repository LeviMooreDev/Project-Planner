using UnityEditor;

namespace ProjectPlanner
{
    public class Spinner
    {
        private readonly double updateDelay = .1d;
        private double nextUpdateTime;
        private int count;

        public void Reset()
        {
            count = 0;
            nextUpdateTime = EditorApplication.timeSinceStartup + updateDelay;
        }

        public void Update()
        {
            if (EditorApplication.timeSinceStartup >= nextUpdateTime)
            {
                nextUpdateTime = EditorApplication.timeSinceStartup + updateDelay;
                count++;
                if (count == 12)
                {
                    count = 0;
                }
            }
        }

        public void Draw()
        {
            switch (count)
            {
                case 0:
                    UI.Image(UI.Icons.Spinner0);
                    break;
                case 1:
                    UI.Image(UI.Icons.Spinner1);
                    break;
                case 2:
                    UI.Image(UI.Icons.Spinner2);
                    break;
                case 3:
                    UI.Image(UI.Icons.Spinner3);
                    break;
                case 4:
                    UI.Image(UI.Icons.Spinner4);
                    break;
                case 5:
                    UI.Image(UI.Icons.Spinner5);
                    break;
                case 6:
                    UI.Image(UI.Icons.Spinner6);
                    break;
                case 7:
                    UI.Image(UI.Icons.Spinner7);
                    break;
                case 8:
                    UI.Image(UI.Icons.Spinner8);
                    break;
                case 9:
                    UI.Image(UI.Icons.Spinner9);
                    break;
                case 10:
                    UI.Image(UI.Icons.Spinner10);
                    break;
                case 11:
                    UI.Image(UI.Icons.Spinner11);
                    break;
            }
        }
    }
}
