using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;
using Quartz;
using Quartz.Impl;

namespace MainFormForHotel
{
    public partial class Form1 : Form
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private IScheduler scheduler;
        public Form1()
        {
            InitializeComponent();
            //  InitializeTrayIcon();
            datePickerRange1.Value = initdate();
        }
        private void InitializeTrayIcon()
        {
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("手动同步", null, OnManualSync);
            trayMenu.Items.Add("退出", null, OnExit);

            trayIcon = new NotifyIcon
            {
                Text = "数据同步工具",
                Icon = new Icon(SystemIcons.Application, 40, 40),
                ContextMenuStrip = trayMenu,
                Visible = true
            };

            trayIcon.DoubleClick += (s, e) => this.Show();
        }

        private async void StartScheduler()
        {
            StdSchedulerFactory factory = new StdSchedulerFactory();
            scheduler = await factory.GetScheduler();
            await scheduler.Start();

            //IJobDetail job = JobBuilder.Create<SyncJob>().WithIdentity("syncJob", "group1").Build();
            //ITrigger trigger = TriggerBuilder.Create()
            //    .WithIdentity("syncTrigger", "group1")
            //    .WithCronSchedule("0 0 1 * * ?") // 每天凌晨1点执行
            //    .Build();

            //await scheduler.ScheduleJob(job, trigger);
        }

        private async void OnManualSync(object sender, EventArgs e)
        {
            //await DataSync.SyncOracleToMySQL();
            MessageBox.Show("数据同步完成！");
        }

        private void OnExit(object sender, EventArgs e)
        {
            trayIcon.Visible = false;
            scheduler?.Shutdown();
            Application.Exit();
        }



        private void exe_data_retrieve_btn_Click(object sender, EventArgs e)
        {
            Console.WriteLine("当前日期（使用DateTime.Today）: ");
            // Wind.SyncOracleToMySQL();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string mysqlConnStr = "Server=127.0.0.1;Port=3306;Database=hotel;User Id=root;Password=123456;";
            string oracleConnStr = "User Id=sys;Password=Orcl$1mph0ny;Data Source=172.16.139.12:1521/mcrspos;DBA Privilege=SYSDBA;";
            try
            {
                using (var conn = new MySqlConnection(mysqlConnStr))
                {
                    conn.Open();
                    Console.WriteLine("✅ MySQL 连接成功！");
                    System.Diagnostics.Debug.WriteLine($"✅ MySQL 连接成功！");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    using (var conn = new MySqlConnection(mysqlConnStr))
                    {
                        conn.Open();
                        Console.WriteLine("OK");
                        System.Diagnostics.Debug.WriteLine($"✅ MySQL 连接成功！");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("error");
                    System.Diagnostics.Debug.WriteLine($"❌ MySQL 连接失败: {exception.Message}");
                }

            }
            try
            {
                using (var conn = new OracleConnection(oracleConnStr))
                {
                    conn.Open();
                    Console.WriteLine("✅ Oracle 连接成功！");
                    System.Diagnostics.Debug.WriteLine($"✅ Oracle 连接成功！1");
                }
            }
            catch (Exception ex)
            {
                try
                {
                    using (var conn = new OracleConnection(oracleConnStr))
                    {
                        conn.Open();
                        Console.WriteLine("OK");
                        System.Diagnostics.Debug.WriteLine($"✅ Oracle 连接成功2！");
                    }
                }
                catch (Exception exception)
                {
                    Console.WriteLine("error");
                    System.Diagnostics.Debug.WriteLine($"❌ Oracle 连接失败: {exception.Message}");
                }

            }

        }

        private void button2_Click(object sender, EventArgs e)
        {

        }

        private void exit_btn_Click(object sender, EventArgs e)
        {
            this.Close();

        }

        private void exesync_btn_Click(object sender, EventArgs e)
        {
            exesync_btn.Enabled = false;

            try
            {
                //Dlt dlt = new Dlt();
                //dlt.SyncData();
            }
            catch { }
            finally
            {
                exesync_btn.Enabled = true;
            }


        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
        public DateTime[] initdate()
        {
            return new DateTime[2] { DateTime.Now.AddDays(-1),
          DateTime.Now.AddDays(-1) };
        }

        private void Form1_Activated(object sender, EventArgs e)
        {
            datePickerRange1.Value = initdate();
        }
    }
}
