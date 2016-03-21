using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace XMLEditor
{
    public partial class Form1 : Form
    {
        string appPath, folderPath;
        string filename = "TestData";
        SQLDatabase db;
        SQLiteConnection connection;

        public Form1()
        {
            InitializeComponent();
            createDataPath();
            db = new SQLDatabase();
        }

        public void createDataPath()
        {
            appPath = Path.GetDirectoryName(Application.ExecutablePath);  // get the root path of the dir
            folderPath = Path.Combine(appPath, "AppData");                // get the path to the AppData folder

            if (!System.IO.Directory.Exists(folderPath))
            {
                MessageBox.Show("AppData folder not found. It will be created automatically");
                System.IO.Directory.CreateDirectory(folderPath);
            }
        }

        private void editCategoryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            db.createDatabase(filename, appPath);               // create database if not exists
            filename = filename + ".sqlite";
            connection = db.createDBConnection(Path.Combine(folderPath, filename), connection);
            db.connectToDB(connection);
            db.createDataTable(connection);
            /*
            string daily = "insert into dailyExpenses (date, transportation, meal, others) values ( @date, @transportation, @meal, @others)";
            SQLiteCommand command = new SQLiteCommand(daily, connection);
            this.Cursor = Cursors.WaitCursor;
            AddButton.Enabled = false;
            //SQLiteTransaction trans = connection.BeginTransaction();
            db.addDataToDailyDB(date, transportation, meal, others, command);
             * */
            //trans.Commit();
            db.closeConnection(connection);
        }
    }
}
