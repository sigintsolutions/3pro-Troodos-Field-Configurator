using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace _3proFieldConfigurator.Tools
{
    public static class DataGridView
    {
        public static void AutoResizeDataGridView(System.Windows.Forms.DataGridView dgv)
        {
            int size = (dgv.Width - dgv.RowHeadersWidth) / dgv.Columns.Count;
            for (int i = 0; i < dgv.Columns.Count; i++)
            {
                if (i != dgv.Columns.Count - 1)
                {
                    dgv.Columns[i].Width = size;
                }
                else
                {
                    dgv.Columns[i].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
                }
            }
        }

        public static void InitializeDgvWithAllClassProperties(System.Type type, System.Windows.Forms.DataGridView dgv)
        {
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            for (int i = 0; i < properties.Length; i++)
            {
                DataGridViewColumn dgvc = new DataGridViewTextBoxColumn
                {
                    HeaderText = properties[i].Name
                };
                dgv.Columns.Add(dgvc);
            }
        }
    }
}
