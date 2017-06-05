using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using System.Collections;

namespace ConvertTablestolatex
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //0
            string aligntopapersize0 = "\\resizebox{\\textwidth}{!}{%\n";
            string aligntopapersize1 = "}";
            if (checkedListBox1.GetItemCheckState(0) != CheckState.Checked)
            {
                aligntopapersize0 = "";
                aligntopapersize1 = "";
            }

            //!
            string columntype = "D";
            string customsizedcolumns = "\\newcolumntype{D}{>{\\centering\\arraybackslash}m{0.14\\textwidth}}\n\\newcolumntype{B}{>{\\centering\\arraybackslash}m{ 0.17\\textwidth}}\n\\centering\n";
            if (checkedListBox1.GetItemCheckState(1) == CheckState.Checked)
            {
                customsizedcolumns = "";
                columntype = "c";
            }

            string str = textBox1.Text;

            string columns = "";
            char b = '0';
            int i = 0;
            int countcolumns = 0;
            while (b != '\n')
            {
                b = str[i];
                if (b == '\t')
                {
                    countcolumns++;
                    columns += (columntype + " ");
                }
                i++;
            }
            columns += (columntype + " ");
            

            string startstr = "\\begin{table}[h!]\n"+ customsizedcolumns+"\\caption{"
               + textBox3.Text + "}\n\\centering\n" + aligntopapersize0 + "\\begin{tabular}{| "
               + columns + "|}\\hline\\hline\n";
            string endstr = "\\end{tabular}"+ aligntopapersize1 +"\n\\label{table:" + textBox4.Text +"}\n\\end{table}\n";

            string strlatex = "";
            string strlatex2 = "";
            strlatex = str.Replace("\t", "$ & $");
            string output = "";
            foreach (string line in new MiscUtil.IO.LineReader(() => new StringReader(strlatex)))
            {
                string my1 = line.Insert(0, "$");
                output += my1.Insert(my1.Length, "$ \\\\\\hline" + Environment.NewLine);
            }
            strlatex2 = output.Replace("$$", "");
            textBox2.Text = startstr + strlatex2 + endstr;
        }

        private void textBox1_Click(object sender, EventArgs e)
        {
            textBox1.SelectAll();
        }

        private void textBox2_Click(object sender, EventArgs e)
        {
            textBox2.SelectAll();
        }

        private void textBox4_Click(object sender, EventArgs e)
        {
            textBox4.SelectAll();
        }

        private void textBox3_Click(object sender, EventArgs e)
        {
            textBox3.SelectAll();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string[] lines = System.IO.File.ReadAllLines("config.cfg");
            List<char> selected = new List<char>();
            for(int i = 0; i<lines.Count();i++)
            {
                string[] tline = lines[i].Split(';');
                selected.Add(tline[0][0]);
                lines[i] = tline[1];
            }
            checkedListBox1.Items.AddRange(lines);
            checkedListBox1.CheckOnClick = true;
            for (int i = 0; i <= (checkedListBox1.Items.Count - 1); i++)
            {
                if (selected[i] == '1')
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                }
                else
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                }
            }
        }
    }
}

namespace MiscUtil.IO
{
    /// <summary>
    /// Reads a data source line by line. The source can be a file, a stream,
    /// or a text reader. In any case, the source is only opened when the
    /// enumerator is fetched, and is closed when the iterator is disposed.
    /// </summary>
    public sealed class LineReader : IEnumerable<string>
    {
        /// <summary>
        /// Means of creating a TextReader to read from.
        /// </summary>
        readonly Func<TextReader> dataSource;

        /// <summary>
        /// Creates a LineReader from a stream source. The delegate is only
        /// called when the enumerator is fetched. UTF-8 is used to decode
        /// the stream into text.
        /// </summary>
        /// <param name="streamSource">Data source</param>
        public LineReader(Func<Stream> streamSource)
            : this(streamSource, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Creates a LineReader from a stream source. The delegate is only
        /// called when the enumerator is fetched.
        /// </summary>
        /// <param name="streamSource">Data source</param>
        /// <param name="encoding">Encoding to use to decode the stream
        /// into text</param>
        public LineReader(Func<Stream> streamSource, Encoding encoding)
            : this(() => new StreamReader(streamSource(), encoding))
        {
        }

        /// <summary>
        /// Creates a LineReader from a filename. The file is only opened
        /// (or even checked for existence) when the enumerator is fetched.
        /// UTF8 is used to decode the file into text.
        /// </summary>
        /// <param name="filename">File to read from</param>
        public LineReader(string filename)
            : this(filename, Encoding.UTF8)
        {
        }

        /// <summary>
        /// Creates a LineReader from a filename. The file is only opened
        /// (or even checked for existence) when the enumerator is fetched.
        /// </summary>
        /// <param name="filename">File to read from</param>
        /// <param name="encoding">Encoding to use to decode the file
        /// into text</param>
        public LineReader(string filename, Encoding encoding)
            : this(() => new StreamReader(filename, encoding))
        {
        }

        /// <summary>
        /// Creates a LineReader from a TextReader source. The delegate
        /// is only called when the enumerator is fetched
        /// </summary>
        /// <param name="dataSource">Data source</param>
        public LineReader(Func<TextReader> dataSource)
        {
            this.dataSource = dataSource;
        }

        /// <summary>
        /// Enumerates the data source line by line.
        /// </summary>
        public IEnumerator<string> GetEnumerator()
        {
            using (TextReader reader = dataSource())
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    yield return line;
                }
            }
        }

        /// <summary>
        /// Enumerates the data source line by line.
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}


