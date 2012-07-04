using System;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
namespace HashCheck
{
	public class MainForm : Form
	{
		private string FileName = "";
		private SQLiteConnection dbcon;
		private IContainer components;
		private Label label3;
		private Label label2;
		private Button button3;
		private Button button2;
		private ProgressBar progressBar1;
		private Button button1;
		private Label label1;
        private OpenFileDialog openFileDialog1;
        System.Threading.Thread workThread;
		public MainForm()
		{
			this.InitializeComponent();
		}
		private void Button1Click(object sender, EventArgs e)
		{
			this.openFileDialog1.CheckFileExists = true;
			if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
			{
				this.FileName = this.openFileDialog1.FileName;
				this.label1.Text = Path.GetFileName(this.FileName);
			}
		}
		private void Button2Click(object sender, EventArgs e)
		{
            if (workThread != null && workThread.ThreadState == System.Threading.ThreadState.Suspended)
                workThread.Resume();
            else
            {
                if (this.FileName != "" && File.Exists(this.FileName))
                {
                    workThread = new System.Threading.Thread(runQuery);
                    workThread.Start();
                    return;
                }
                else
                {
                    MessageBox.Show("Database not Found", "Error", MessageBoxButtons.OK);
                }
            }
		}

        private void runQuery()
        {
            try
            {
                this.dbcon = new SQLiteConnection("Data Source=" + this.FileName);
                using (SQLiteCommand sQLiteCommand = this.dbcon.CreateCommand())
                {
                    this.dbcon.Open();
                    using (SQLiteTransaction sQLiteTransaction = this.dbcon.BeginTransaction())
                    {
                        sQLiteCommand.CommandText = "CREATE TABLE IF NOT EXISTS cracked_passwords (MD5 VARCHAR PRIMARY KEY,plain VARCHAR)";
                    lolo:
                        try
                        {
                            sQLiteCommand.ExecuteNonQuery();
                            sQLiteTransaction.Commit();
                        }
                        catch (SQLiteException e)
                        {
                            if (e.ErrorCode == SQLiteErrorCode.Busy)
                            {
                                System.Threading.Thread.Sleep(200);
                                goto lolo;
                            }
                        }
                    }
                }
                using (SQLiteCommand sQLiteCommand2 = this.dbcon.CreateCommand())
                {
                    sQLiteCommand2.CommandText = "SELECT Count(DISTINCT passhash) FROM users WHERE (SELECT MD5 FROM cracked_passwords WHERE MD5 = passhash) IS NULL";
                    DbDataReader max = sQLiteCommand2.ExecuteReader();
                    int maximum = 0;
                    if (max.HasRows)
                    {
                        max.Read();
                        maximum = int.Parse(max.GetValue(0).ToString());
                    }
                    max.Close();
                    max = null;
                    sQLiteCommand2.CommandText = "SELECT DISTINCT passhash FROM users WHERE (SELECT MD5 FROM cracked_passwords WHERE MD5 = passhash) IS NULL;";
                    using (DbDataReader dbDataReader = sQLiteCommand2.ExecuteReader())
                    {
                        bool flag = false;
                        MD5crack mD5crack = new MD5crack_hashkiller();
                        MD5crack mD5crack2 = new MD5crack_c0llision();
                        this.progressBar1.Maximum = maximum;
                        this.label3.Text = "Total: " + maximum.ToString();
                        int num = 0;
                        do
                        {
                            flag = dbDataReader.Read();
                            num++;
                            this.progressBar1.Value = num;
                            this.label2.Text = "Current: " + num.ToString();
                            string text = dbDataReader.GetValue(0).ToString();
                            if (/*!this.AllReadyDone(text)*/true)
                            {
                                MD5crack_result mD5crack_result = mD5crack.crack(text);
                                if (!mD5crack_result.isCracked())
                                {
                                    mD5crack_result = mD5crack2.crack(text);
                                }
                                if (mD5crack_result.isCracked())
                                {
                                    using (SQLiteCommand sQLiteCommand3 = this.dbcon.CreateCommand())
                                    {
                                        sQLiteCommand3.CommandText = "INSERT OR REPLACE INTO cracked_passwords (MD5,plain) VALUES (@md5,@plain)";
                                        sQLiteCommand3.Parameters.AddWithValue("md5", text.ToLower());
                                        sQLiteCommand3.Parameters.AddWithValue("plain", mD5crack_result.getClear());
                                    insertpwd:
                                        try
                                        {
                                            sQLiteCommand3.ExecuteNonQuery();
                                        }
                                        catch (SQLiteException e)
                                        {
                                            if (e.ErrorCode == SQLiteErrorCode.Busy)
                                            {
                                                System.Threading.Thread.Sleep(200);
                                                goto insertpwd;
                                            }
                                        }

                                    }
                                }
                            }
                        }
                        while (flag);
                    }
                    return;
                }
            }
            catch (System.Threading.ThreadAbortException ex)
            {
                MessageBox.Show(ex.Message);
                this.dbcon.Close();
                this.dbcon = null;
            }
        }
		private bool AllReadyDone(string md5)
		{
			bool hasRows = false;
			using (SQLiteCommand sQLiteCommand = this.dbcon.CreateCommand())
			{
				sQLiteCommand.CommandText = "SELECT MD5,plain FROM cracked_passwords WHERE MD5 LIKE @md5 LIMIT 1";
				sQLiteCommand.Parameters.AddWithValue("md5", md5);
            queryexisting:
                try
                {
                    using (DbDataReader dbDataReader = sQLiteCommand.ExecuteReader())
                    {
                        hasRows = dbDataReader.HasRows;
                    }
                }
                catch (SQLiteException e)
                {
                    if (e.ErrorCode == SQLiteErrorCode.Busy)
                    {
                        System.Threading.Thread.Sleep(200);
                        goto queryexisting;
                    }
                }
			}
			return hasRows;
		}
		protected override void Dispose(bool disposing)
		{
			if (disposing && this.components != null)
			{
				this.components.Dispose();
			}
			base.Dispose(disposing);
		}
		private void InitializeComponent()
		{
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.button2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(18, 26);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(100, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Database File";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(144, 26);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "select";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.Button1Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(12, 113);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(260, 23);
            this.progressBar1.TabIndex = 2;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(23, 190);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(75, 23);
            this.button2.TabIndex = 3;
            this.button2.Text = "Start";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.Button2Click);
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(164, 190);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(75, 23);
            this.button3.TabIndex = 4;
            this.button3.Text = "Cancel";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(12, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 23);
            this.label2.TabIndex = 5;
            this.label2.Text = "label2";
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(12, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "label3";
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "db";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "databases|*.db;*.db3|All FIles|*";
            this.openFileDialog1.SupportMultiDottedExtensions = true;
            // 
            // MainForm
            // 
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "MainForm";
            this.Text = "HashCheck";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.ResumeLayout(false);

		}

        private void button3_Click(object sender, EventArgs e)
        {
            if (workThread != null && workThread.ThreadState != System.Threading.ThreadState.Suspended)
                workThread.Suspend();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(workThread!=null)
                workThread.Abort();
            workThread = null;
            if (dbcon != null)
            {
                try
                {
                    dbcon.Close();
                }
                catch (Exception ex)
                {
                }
            }
                dbcon=null;

            
        }
	}
}
