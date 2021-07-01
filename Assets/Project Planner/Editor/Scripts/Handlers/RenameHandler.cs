using System;

namespace ProjectPlanner
{
    public class RenameHandler
    {
        private string ID;
        public bool Active
        {
            get
            {
                return Object != null;
            }
        }
        public IRename Object
        {
            get;
            private set;
        }
        private string Name;
        private readonly Action<Action> safeUI;
        private readonly Action repaint;

        public RenameHandler(Action<Action> safeUI, Action repaint)
        {
            this.safeUI = safeUI;
            this.repaint = repaint;
        }
        public bool Is(IRename obj)
        {
            return obj.Equals(Object);
        }
        public void Begin(IRename value)
        {
            End();

            safeUI.Invoke(() =>
            {
                ID = UnityEngine.Random.value.ToString();
                Object = value;
                Name = value.GetName();

                UI.Utilities.LossFocus();
                repaint.Invoke();
            });
        }
        public void End(bool use = true)
        {
            safeUI.Invoke(() =>
            {
                if (!Active)
                    return;

                if (use)
                {
                    UndoHandler.SaveState("Rename");

                    Object.SetName(Name);
                    Object.Validate();
                }

                Object = default(IRename);
                Name = string.Empty;

                UI.Utilities.LossFocus();
                FileManager.SaveAll();
                repaint.Invoke();
            });
        }
        public void Draw(bool toolbar = false)
        {
            UI.Utilities.Focus("rename_" + ID);
            UI.Utilities.Name("rename_" + ID);
            Name = UI.TextField(Name, toolbar ? UI.TextFieldStyle.Toolbar : UI.TextFieldStyle.Normal);
        }
    }
}