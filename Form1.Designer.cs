namespace DeepNotes
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            recordButton = new Button();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            quitToolStripMenuItem = new ToolStripMenuItem();
            helpToolStripMenuItem = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            checkForUpdatesToolStripMenuItem = new ToolStripMenuItem();
            gitHubPageToolStripMenuItem = new ToolStripMenuItem();
            statusStrip1 = new StatusStrip();
            toolStripStatusLabel1 = new ToolStripStatusLabel();
            splitContainer1 = new SplitContainer();
            timer = new Label();
            textBox1 = new TextBox();
            stopButton = new Button();
            treeView1 = new TreeView();
            volumeSlider1 = new NAudio.Gui.VolumeSlider();
            volumeMeter1 = new NAudio.Gui.VolumeMeter();
            menuStrip1.SuspendLayout();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            SuspendLayout();
            // 
            // recordButton
            // 
            recordButton.Location = new Point(12, 3);
            recordButton.Name = "recordButton";
            recordButton.Size = new Size(75, 23);
            recordButton.TabIndex = 0;
            recordButton.Text = "Record";
            recordButton.UseVisualStyleBackColor = true;
            // 
            // menuStrip1
            // 
            menuStrip1.BackColor = SystemColors.Control;
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, helpToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(800, 24);
            menuStrip1.TabIndex = 3;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { quitToolStripMenuItem });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // quitToolStripMenuItem
            // 
            quitToolStripMenuItem.Name = "quitToolStripMenuItem";
            quitToolStripMenuItem.Size = new Size(97, 22);
            quitToolStripMenuItem.Text = "Quit";
            // 
            // helpToolStripMenuItem
            // 
            helpToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { aboutToolStripMenuItem, checkForUpdatesToolStripMenuItem, gitHubPageToolStripMenuItem });
            helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            helpToolStripMenuItem.Size = new Size(44, 20);
            helpToolStripMenuItem.Text = "Help";
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(170, 22);
            aboutToolStripMenuItem.Text = "About...";
            // 
            // checkForUpdatesToolStripMenuItem
            // 
            checkForUpdatesToolStripMenuItem.Name = "checkForUpdatesToolStripMenuItem";
            checkForUpdatesToolStripMenuItem.Size = new Size(170, 22);
            checkForUpdatesToolStripMenuItem.Text = "Check for updates";
            // 
            // gitHubPageToolStripMenuItem
            // 
            gitHubPageToolStripMenuItem.Name = "gitHubPageToolStripMenuItem";
            gitHubPageToolStripMenuItem.Size = new Size(170, 22);
            gitHubPageToolStripMenuItem.Text = "GitHub Page";
            // 
            // statusStrip1
            // 
            statusStrip1.BackColor = SystemColors.Control;
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripStatusLabel1 });
            statusStrip1.Location = new Point(0, 428);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(800, 22);
            statusStrip1.TabIndex = 3;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            toolStripStatusLabel1.Size = new Size(149, 17);
            toolStripStatusLabel1.Text = "DeepSpeech not initialized.";
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = DockStyle.Fill;
            splitContainer1.Location = new Point(0, 24);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.BackColor = SystemColors.Control;
            splitContainer1.Panel1.Controls.Add(volumeMeter1);
            splitContainer1.Panel1.Controls.Add(volumeSlider1);
            splitContainer1.Panel1.Controls.Add(timer);
            splitContainer1.Panel1.Controls.Add(textBox1);
            splitContainer1.Panel1.Controls.Add(stopButton);
            splitContainer1.Panel1.Controls.Add(recordButton);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.BackColor = Color.FromArgb(207, 198, 176);
            splitContainer1.Panel2.Controls.Add(treeView1);
            splitContainer1.Size = new Size(800, 404);
            splitContainer1.SplitterDistance = 266;
            splitContainer1.TabIndex = 1;
            // 
            // timer
            // 
            timer.AutoSize = true;
            timer.Location = new Point(93, 7);
            timer.Name = "timer";
            timer.Size = new Size(34, 15);
            timer.TabIndex = 3;
            timer.Text = "00:00";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(3, 144);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(261, 257);
            textBox1.TabIndex = 2;
            // 
            // stopButton
            // 
            stopButton.Enabled = false;
            stopButton.Location = new Point(12, 32);
            stopButton.Name = "stopButton";
            stopButton.Size = new Size(75, 23);
            stopButton.TabIndex = 1;
            stopButton.Text = "Stop";
            stopButton.UseVisualStyleBackColor = true;
            stopButton.Click += stopButton_Click;
            // 
            // treeView1
            // 
            treeView1.BackColor = SystemColors.Control;
            treeView1.Dock = DockStyle.Fill;
            treeView1.Location = new Point(0, 0);
            treeView1.Name = "treeView1";
            treeView1.Size = new Size(530, 404);
            treeView1.TabIndex = 0;
            // 
            // volumeSlider1
            // 
            volumeSlider1.Location = new Point(12, 61);
            volumeSlider1.Name = "volumeSlider1";
            volumeSlider1.Size = new Size(96, 16);
            volumeSlider1.TabIndex = 5;
            // 
            // volumeMeter1
            // 
            volumeMeter1.Amplitude = 0F;
            volumeMeter1.Location = new Point(12, 83);
            volumeMeter1.MaxDb = 18F;
            volumeMeter1.MinDb = -60F;
            volumeMeter1.Name = "volumeMeter1";
            volumeMeter1.Size = new Size(96, 23);
            volumeMeter1.TabIndex = 6;
            volumeMeter1.Text = "volumeMeter1";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.Control;
            ClientSize = new Size(800, 450);
            Controls.Add(splitContainer1);
            Controls.Add(menuStrip1);
            Controls.Add(statusStrip1);
            MainMenuStrip = menuStrip1;
            Name = "Form1";
            Text = "Form1";
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel1.PerformLayout();
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
            splitContainer1.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button recordButton;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem quitToolStripMenuItem;
        private ToolStripMenuItem helpToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private ToolStripMenuItem checkForUpdatesToolStripMenuItem;
        private ToolStripMenuItem gitHubPageToolStripMenuItem;
        private StatusStrip statusStrip1;
        private ToolStripStatusLabel toolStripStatusLabel1;
        private SplitContainer splitContainer1;
        private TreeView treeView1;
        private Button stopButton;
        private TextBox textBox1;
        private Label timer;
        private NAudio.Gui.VolumeMeter volumeMeter1;
        private NAudio.Gui.VolumeSlider volumeSlider1;
    }
}