using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ScreenDebug
{
    public partial class FrmMain : Form
    {
        public FrmMain()
        {
            InitializeComponent();
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void btnGetDeviceName_Click(object sender, EventArgs e)
        {
            var sm = new ScreenManager();
            sm.Init();
            if (listBox1.Items.Count != 0)
            {
                listBox1.Items.Add(Environment.NewLine);
            }
            foreach (var s in sm.ScreensInfo)
            {
                listBox1.Items.Add($"[{DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss fff")}]{s.ToString()}");
            }
            
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }
    }

    public class ScreenManager
    {
        private List<ScreenInfo> _lst = new List<ScreenInfo>();
        public List<ScreenInfo> GetRuntimeScreenConfig()
        {
            var lst = new List<ScreenInfo>();
            for (var i = 0; i < Screen.AllScreens.Length; i++)
            {
                lst.Add(new ScreenInfo
                {
                    ScreenNo = i,
                    DeviceName = Screen.AllScreens[i].DeviceName,
                    IsPrimaryScreen = Screen.AllScreens[i].Primary
                });
            }

            return lst;
        }

        public void Init()
        {
            _lst = GetRuntimeScreenConfig();
        }

        /// <summary>
        /// 检测运行时，屏幕配置是否有变化
        /// </summary>
        /// <returns></returns>
        public bool IsRuntimeScreenConfig()
        {
            var lstRuntime = GetRuntimeScreenConfig();
            if (_lst.Count != lstRuntime.Count)
            {
                return true;
            }
            foreach (var s in _lst)
            {
                if (!lstRuntime.Any(x => x.ScreenNo == s.ScreenNo && x.DeviceName == s.DeviceName))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 返回控件所在的屏幕索引
        /// </summary>
        /// <param name="c"></param>
        /// <returns></returns>
        public int GetScreenNoByControl(Control c)
        {
            // 根据当前组件的位置查找组件所在屏幕的索引
            var sc = _lst.FirstOrDefault(x => x.DeviceName == Screen.FromControl(c).DeviceName);
            if (sc != null)
            {
                return sc.ScreenNo;
            }
            else
            {
                // 未找到，需要重新扫描显示器配置
                return 0;
            }
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            _lst.Sort(new ScreenInfoCompare());
            for (var i = 0; i < _lst.Count; i++)
            {
                sb.AppendLine(_lst[i].ToString());
            }
            return sb.ToString();
        }

        public IEnumerable<ScreenInfo> ScreensInfo => _lst;

        private class ScreenInfoCompare : IComparer<ScreenInfo>
        {
            public int Compare(ScreenInfo x, ScreenInfo y)
            {
                if (x.ScreenNo < y.ScreenNo)
                    return -1;
                else if (x.ScreenNo == y.ScreenNo)
                    return 0;
                else
                    return 1;
            }
        }
    }

    public class ScreenInfo
    {
        /// <summary>
        /// 显示器的设备名
        /// </summary>
        public string DeviceName { get; set; }
        /// <summary>
        /// 显示器编号
        /// </summary>
        public int ScreenNo { get; set; }
        /// <summary>
        /// 是否是默认显示器
        /// </summary>
        public bool IsPrimaryScreen { get; set; }

        public override string ToString()
        {
            return $"ScreenNo={ScreenNo}, DeviceName={DeviceName}, IsPrimaryScreen={IsPrimaryScreen}";
        }
    }
}
