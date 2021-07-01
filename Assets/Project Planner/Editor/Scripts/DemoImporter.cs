using UnityEditor;
using UnityEngine;

namespace ProjectPlanner
{
    public static class DemoImporter
    {
        public static void Init()
        {
            if (UI.Utilities.Dialog(Language.ImportDemoContent, Language.AreYouSureYouWantToImportTheDemoContentNoExistingDataWillBeOverwritten, Language.Import, Language.No))
            {
                Import();
            }
        }

        private static readonly string prefix = "DEMO";

        private static void Import()
        {
            UndoHandler.SaveState("Import Demo Content");

            #region Tags
            string betaContentTagId = Tags.I.Add(prefix + "-Beta Content");
            string programmingTagId = Tags.I.Add(prefix + "-Programming");
            Tags.I.Add(prefix + "-Important");
            string artworkTagId = Tags.I.Add(prefix + "-Artwork");
            string bugTagId = Tags.I.Add(prefix + "-Bug");
            #endregion

            #region Colors
            string yellowId = Colors.I.Add(Color.yellow);
            string greenId = Colors.I.Add(Color.green);
            string whiteId = Colors.I.Add(Color.white);
            string blueId = Colors.I.Add(new Color(0, 0.39f, 1, 1));
            string redId = Colors.I.Add(Color.red);
            #endregion

            #region Board
            Board board = Boards.I.New(false);
            board.Name = prefix;
            Boards.I.ValidateBoardNames();
            #endregion

            #region This is a group
            {
                Group group = Groups.I.New(board);
                group.Name = "This is a group";
                Groups.I.ValidateNames(board);

                Task gt1 = Tasks.I.New(group, false);
                gt1.Name = "Thanks for downloading Project Planner. We hope you will find it useful.";
                gt1.ColorIds.AddRange(new string[] { redId, greenId, blueId });

                Task gt2 = Tasks.I.New(group, false);
                gt2.Name = "This is a task";

                Task gt3 = Tasks.I.New(group, false);
                gt3.Name = "A group can contain as many tasks as you want and is an easy way to keep things organized";

                Task gt4 = Tasks.I.New(group, false);
                gt4.Name = "Try to click on this task";
                gt4.Description = "You can write a more detailed description for a task here. \n\nYou can assign assets, tags, colors and subtasks to a task.You can learn more about the features of Project Planner by reading the tasks under the \"Task features\" group.";
            }
            #endregion

            #region Task features
            {
                Group group = Groups.I.New(board);
                group.Name = "Task features";
                Groups.I.ValidateNames(board);

                Task gt1 = Tasks.I.New(group, false);
                gt1.Name = "Tags";
                gt1.Description = "You access tags by clicking on the Tags icon below the description. \n\nTags are shared between all tasks and boards. If you edit or delete a tag it will affect every task with that tag. \n\nYou can not have more than one tag with the same name. Tags are not case sensitive. \n\nA tag is assigned to a task when there is a checkmark next to it. \n\nYou can filter what tasks to show in the board window by clicking on the \"Search by Tag\" button next to the search bar. For a task to show it has to contain all the selected tags.";

                Task gt2 = Tasks.I.New(group, false);
                gt2.Name = "Colors";
                gt2.Description = "You access colors by clicking on the Colors icon below the description. \n\nColors behave very much in the same way as tags.You select, add, edit, delete and search the same way. \n\nWhat is unique to colors is that they are displayed with the task in the board window.This makes them useful for creating an easy to understand overview and similar things.";

                Task gt3 = Tasks.I.New(group, false);
                gt3.Name = "Assigning Assets";
                gt3.Description = "You can assign as many assets from your project to a task as you want. \n\nAdd: click on the circle next to the empty object field. \nReplace: click on the circle next to the asset you want to replace. \nRemove: click on the asset and press the delete key. \n\nWhen you click on an asset it will be highlighted and selected in the Project view.";
                gt3.ShowAssets = true;

                Task gt4 = Tasks.I.New(group, false);
                gt4.Name = "Moving";
                gt4.Description = "There are two ways of moving a task around. \n1.Right clicking on a task will show the options to move it up or down. \n2.Mouse drag it to where you want it to be. \n\nIt is not possible to move a task to another board in the current version of Project Planner.";

                Task gt5 = Tasks.I.New(group, false);
                gt5.Name = "Subtasks";
                gt5.Description = "You can split a task into smaller subtasks to help you keep everything more organized. \n\nAll subtasks are shown in a list at the bottom of the task window. \n\nA subtask behaves just like a normal task except that it can not have tags assigned to it. \n\nWhen you are done with a subtask you can mark it as completed with the checkbox next to it. If a subtask has its own tasks the checkbox is automatically checked when all of its own subtasks are completed. You can hide completed tasks by clicking on the eye.";
                gt5.ShowSubtasks = true;

                Task gt5s1 = Tasks.I.New(gt5, false);
                gt5s1.Name = "I am a subtask";

                Task gt5s2 = Tasks.I.New(gt5, false);
                gt5s2.Name = "I am a completed subtask";
                gt5s2.Done = true;

                Task gt5s3 = Tasks.I.New(gt5, false);
                gt5s3.Name = "I have my own subtask";

                Task gt5s3s1 = Tasks.I.New(gt5s3, false);
                gt5s3s1.Name = "Task";

                Task gt5s3s2 = Tasks.I.New(gt5s3, false);
                gt5s3s2.Name = "Task";

                Task gt5s3s3 = Tasks.I.New(gt5s3, false);
                gt5s3s3.Name = "Task";

                Task gt5s4 = Tasks.I.New(gt5, false);
                gt5s4.Name = "Click on me to read about navigating subtasks";
                gt5s4.Description = "When you have selected a subtask two extra buttons are shown in the toolbar at the top left. \n\nBack: Goes to the parent of the task you are viewing. \nGo To: Shows a list of breadcrumbs all the way back to the root task.";

                Task gt5s4s1 = Tasks.I.New(gt5s4, false);
                gt5s4s1.Name = "Click on me to try the \"Go To\" button";

                Task gt6 = Tasks.I.New(group, false);
                gt6.Name = "Locking tasks";
                gt6.Description = "You can lock tasks and make them uneditable by clicking on the lock icon in the top right corner. To unlock a task click on the same icon again. \n\nWhen a task is locked you can still edit the content of its subtasks, but not move them or mark them as completed. \n\nWhen a task is locked, all the empty fields will be hidden.";
                gt6.Locked = true;

                Task gt6s1 = Tasks.I.New(gt6, false);
                gt6s1.Name = "Task";
                gt6s1.Done = true;
            }
            #endregion

            #region Extra
            {
                Group group = Groups.I.New(board);
                group.Name = "Extra";
                Groups.I.ValidateNames(board);

                Task gt1 = Tasks.I.New(group, false);
                gt1.Name = "Manual";
                gt1.Description = "This demo board contains a quick overview of how to use Project Planner. If you need a more in-depth description of how things work we suggest reading the manual. \n\nYou can find it here \nTools->Project Planner->Help->Manual";

                Task gt2 = Tasks.I.New(group, false);
                gt2.Name = "Settings";
                gt2.Description = "You can easily customize how Project Planner looks and behaves in the settings menu. \n\nThere are two ways of getting to the settings menu. \nTools->Project Planner->Settings \nEdit->Preferences->Project Planner \n\n";
            }
            #endregion

            #region Tasks for searching
            {
                Group group = Groups.I.New(board);
                group.Name = "Tasks for searching";
                Groups.I.ValidateNames(board);

                Task gt1 = Tasks.I.New(group, false);
                gt1.Name = "I have tags Artwork and Bug";
                gt1.TagIds.AddRange(new string[] { artworkTagId, bugTagId });

                Task gt2 = Tasks.I.New(group, false);
                gt2.Name = "I have tags Bug and Programming";
                gt2.TagIds.AddRange(new string[] { programmingTagId, bugTagId });

                Task gt3 = Tasks.I.New(group, false);
                gt3.Name = "I have tags Artwork and Beta Content";
                gt3.TagIds.AddRange(new string[] { artworkTagId, betaContentTagId });

                Task gt4 = Tasks.I.New(group, false);
                gt4.Name = "I have no tags";

                Task gt5 = Tasks.I.New(group, false);
                gt5.Name = "I have tag Bug";
                gt5.TagIds.AddRange(new string[] { bugTagId });

                Task gt6 = Tasks.I.New(group, false);
                gt6.Name = "------";

                Task gt7 = Tasks.I.New(group, false);
                gt7.Name = "Red and green";
                gt7.ColorIds.AddRange(new string[] { redId, greenId });

                Task gt8 = Tasks.I.New(group, false);
                gt8.Name = "Yellow and green";
                gt8.ColorIds.AddRange(new string[] { yellowId, greenId });

                Task gt9 = Tasks.I.New(group, false);
                gt9.Name = "Blue and red";
                gt9.ColorIds.AddRange(new string[] { blueId, redId });

                Task gt10 = Tasks.I.New(group, false);
                gt10.Name = "White";
                gt10.ColorIds.AddRange(new string[] { whiteId });
            }
            #endregion

            UI.Utilities.Dialog(Language.ImportDemoContent, Language.DemoContentWasImportedSuccessfully, Language.OK);
            if (BoardWindow.I != null)
                BoardWindow.I.currentBoardIndex = Boards.I.All.IndexOf(board);
        }
    }
}