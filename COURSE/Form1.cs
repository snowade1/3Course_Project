using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using Microsoft.VisualBasic;
using System.IO;
using System.Runtime.CompilerServices;

namespace COURSE
{
    public partial class Form1 : Form
    {
        private List<Process> processes = null;

        public Form1()
        {
            InitializeComponent();
        }

        private void GetProcesses()
        {
            processes.Clear();
            processes = Process.GetProcesses().ToList<Process>();
        }

        private void UpdateProcesses()
        {
            listView1.Items.Clear();
            double memsize = 0;
            foreach (Process process in processes)
            {
                memsize = 0;
                PerformanceCounter PC = new PerformanceCounter();
                PC.CategoryName = "Process";
                PC.CounterName = "Working Set - Private";
                PC.InstanceName = process.ProcessName;
                memsize = (double)PC.NextValue() / (1024 * 1024);
                string[] row = new string[] { process.ProcessName.ToString(), Math.Round(memsize, 1).ToString(), process.Id.ToString(), process.MainWindowTitle.ToString() };
                listView1.Items.Add(new ListViewItem(row));
                process.Close();
                process.Dispose();
            }
            Text = "Запущено " + processes.Count.ToString() + " процессов!";
        }

        private void KillProcess(Process process)
        {
            DateTime date = DateTime.Now;
            process.Kill();
            process.WaitForExit();
            string[] row = new string[] { date.ToString(), "ЗАВЕРШЕНИЕ: " + process.ProcessName.ToString() + " был завершён!" };
            listView2.Items.Add(new ListViewItem(row));
        }

        private void KillProcessChildren(Process process)
        {
            string name = process.ProcessName.ToString();
            System.Diagnostics.Process[] etc = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process anti in etc)
            {
                try
                {
                    if (anti.ProcessName.ToLower().Contains(name.ToLower()))
                    {
                        anti.Kill();
                        anti.WaitForExit();
                        DateTime date = DateTime.Now;
                        string[] row = new string[] { date.ToString(), "ЗАВЕРШЕНИЕ: " + anti.ProcessName.ToString() + " был завершён (древо)!" };
                        listView2.Items.Add(new ListViewItem(row));
                    }
                }
                catch (ArgumentException) { }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            processes = new List<Process>();
            GetProcesses();
            UpdateProcesses();
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            GetProcesses();
            UpdateProcesses();
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя программы:", "Запуск нового процесса");
            try
            {
                DateTime date = DateTime.Now;
                Process.Start(path);
                string[] row = new string[] { date.ToString(), "ЗАПУСК: " + path.ToString() + " был запущен!" };
                listView2.Items.Add(new ListViewItem(row));
            }
            catch (Exception) { }
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя процесса:", "Завершение работы процесса");
            try
            {
                foreach (Process proc in Process.GetProcessesByName(path))
                {
                    KillProcess(proc);
                    GetProcesses();
                    UpdateProcesses();
                }
            }
            catch (Exception) { }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя процесса:", "Завершение дерева процессов");
            try
            {
                foreach (Process proc in Process.GetProcessesByName(path))
                {
                    KillProcessChildren(proc);
                    GetProcesses();
                    UpdateProcesses();
                }
            }
            catch (Exception) { }
        }

        private void завершитьДеревоПроцессовToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя процесса:", "Завершение дерева процессов");
            try
            {
                foreach (Process proc in Process.GetProcessesByName(path))
                {
                    KillProcessChildren(proc);
                    GetProcesses();
                    UpdateProcesses();
                }
            }
            catch (Exception) { }
        }

        private void GetInfo(Process process)
        {
            try
            {
                DateTime date = DateTime.Now;
                string[] row = new string[] { date.ToString(), "СВЕДЕНИЯ: " + process.ProcessName.ToString() + ", Base Priority: " + process.BasePriority + ", Main Window Title: " + process.MainWindowTitle + ", Total Processor Time: " + process.TotalProcessorTime };
                listView2.Items.Add(new ListViewItem(row));
            }
            catch (Exception) { }
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            string path = Interaction.InputBox("Введите имя процесса:", "Сведения о процессе");
            try
            {
                foreach (Process proc in Process.GetProcessesByName(path))
                {
                    GetInfo(proc);
                    GetProcesses();
                    UpdateProcesses();
                }
            }
            catch (Exception) { }
        }
    }
}