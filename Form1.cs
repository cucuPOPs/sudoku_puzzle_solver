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
namespace _0104
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        string[,] values = new string[9, 9];
        
        void display(string[,] values)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    //컨트롤 이름 a1~i9 찾는다.
                    Control[] ctrls = this.Controls.Find(Convert.ToChar('a' + i).ToString() + (j + 1).ToString(), false);
                    if (ctrls != null && ctrls.Length > 0)
                    {
                        TextBox tbox = ctrls[0] as TextBox;
                        tbox.Text = "";
                        tbox.Text = values[i, j];
                    }
                }
            }
        }
        bool Assign(string[,] values, int i, int j, string d)
        {
            //d 를 제외한 모든 값을 지움.//Eliminate 함수사용.
            int num = Convert.ToInt32(d);
            string other_values = values[i, j].Replace(d, "");
            foreach (char s in other_values)
            {
                if (!Eliminate(values, i, j, s.ToString()))
                {
                    return false;//모순 발견시,false.
                }
            }
            //할당 성공시,
            return true;
        }

        //values[i,j]에서 s를 지운다.
        bool Eliminate(string[,] values, int i, int j, string s)
        {
            //만약 두 전략 중 하나에 해당하면
            //적절히 제약 조건을 전파하고, 변경된 values 를 반환한다.
            //만약 모순을 발견하면 False 를 반환한다.
            //string[,] copy = (string[,])this.values.Clone();
            if (!values[i, j].Contains(s))
            {
                return true;//이미 제거됨.
            }
            //제거하자.
            values[i, j] = values[i, j].Replace(s, "");

            //1.
            //어떤 빈 칸에 들어갈 수 있는 숫자가 하나밖에 없다면,
            //해당 칸의 이웃들에는 그 숫자가 들어갈 수 없다.
            if (values[i, j].Length == 0)
            {
                return false;//모순: 후보숫자가 없는경우.
            }
            else if (values[i, j].Length == 1)
            {
                //이웃에 그숫자 제거.
                //제거해야할 숫자.= values[i,j]

                //가로의 이웃
                for (int t = 0; t < 9; t++)
                {
                    if (t == j)
                        continue;
                    //후보숫자에서 s를 찾아 제거.
                    //values[i, t] = values[i, t].Replace(s, "");
                    if (!Eliminate(values, i, t, values[i, j]))
                    {
                        return false;
                    }
                }
                //세로의 이웃
                for (int t = 0; t < 9; t++)
                {
                    if (t == i)
                        continue;
                    //후보숫자에서 s를 찾아 제거.
                    //values[t, j] = values[t, j].Replace(s, "");
                    if (!Eliminate(values, t, j, values[i, j]))
                    {
                        return false;
                    }
                }
                //3x3의 이웃
                for (int t = i / 3 * 3; t < (i / 3 * 3) + 3; t++)
                {
                    for (int tt = j / 3 * 3; tt < (j / 3 * 3) + 3; tt++)
                    {
                        if (t == i && tt == j) continue;
                        //후보숫자에서 s를 찾아 제거.
                        //values[t, tt] = values[t, tt].Replace(s, "");
                        if (!Eliminate(values, t, tt, values[i, j]))
                        {
                            return false;
                        }
                    }
                }
            }



            //2.
            //한 단위에 어떤 숫자가 들어갈 수 있는 칸이 하나밖에 없다면, 
            //거기에 그 숫자를 쓴다.
            //values[i,j]에서 s를 지움.
            //i,j,단위에 s 가 1개뿐인 칸이 있으면, 거기에 s를 쓴다.

            //가로의 이웃
            int cnt = 0, idx = -1;
            for (int t = 0; t < 9; t++)
            {
                if (values[i, t].Contains(s))
                {
                    cnt++;
                    idx = t;
                }
            }
            if (cnt == 1)
            {
                if (!Assign(values, i, idx, s))
                {
                    //MessageBox.Show("asdf");
                    return false;
                }

            }
            cnt = 0; idx = -1;
            //세로의 이웃
            for (int t = 0; t < 9; t++)
            {
                if (values[t, j].Contains(s))
                {
                    cnt++;
                    idx = t;
                }
            }
            if (cnt == 1)
            {
                if (!Assign(values, idx, j, s))
                {
                    //MessageBox.Show("asdf");
                    return false;
                }
            }
            cnt = 0; idx = -1;
            //3x3의 이웃
            int idx2 = -1;
            for (int t = i / 3 * 3; t < (i / 3 * 3) + 3; t++)
            {
                for (int tt = j / 3 * 3; tt < (j / 3 * 3) + 3; tt++)
                {
                    if (values[t, tt].Contains(s))
                    {
                        cnt++;
                        idx = t;
                        idx2 = tt;
                    }
                }
            }
            if (cnt == 1)
            {
                if (!Assign(values, idx, idx2, s))
                {
                    //MessageBox.Show("asdf");
                    return false;
                }
            }

            return true;
        }


        string[,] search(string[,] values)
        {
            int square_count = 0;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (values[i, j] == null)
                        return new string[9, 9];
                    else if (values[i, j].Length == 1)
                        square_count++;


            if (square_count == 81) return values;
            //해결.

            //답을 못찾은칸중 후보의 수가 가장 적은칸 i,j를 찾는다.
            int min_size = 10;
            int save_i = 0, save_j = 0;
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    if (values[i, j].Length > 1 && values[i, j].Length < min_size)
                    {
                        min_size = values[i, j].Length;
                        save_i = i;
                        save_j = j;
                    }
            //후보개수가 가장 적은 칸 i,j를 찾았다. save_i,save_j.
            string[,] copy;


            //후보중 하나를 할당 시도해본다.
            for (int k = 0; k < values[save_i, save_j].Length; k++)
            {
                copy = (string[,])values.Clone();
                if (Assign(copy, save_i, save_j, values[save_i, save_j][k].ToString()))
                {
                    string[,] temp = search(copy);
                    if (temp[0, 0] == null) continue;
                    else return temp;
                }
            }
            //후보 모두 실패시.
            return new string[9, 9];
        }










        //search
        private void button10_Click(object sender, EventArgs e)
        {

            Stopwatch sw = new Stopwatch();
            sw.Start();



            //MessageBox.Show(values[i, j]);

            //values[,] 초기화
            //처음엔 모든칸이 모든 후보를 가진다.
            for (int i = 0; i < 9; i++)
                for (int j = 0; j < 9; j++)
                    values[i, j] = "123456789";


            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    //컨트롤 이름 a1~i9 찾는다.
                    Control[] ctrls = this.Controls.Find(Convert.ToChar('a' + i).ToString() + (j + 1).ToString(), false);
                    if (ctrls != null && ctrls.Length > 0)
                    {
                        TextBox tbox = ctrls[0] as TextBox;
                        if (tbox.Text != "")
                        {
                            //숫자 쓰인칸 발견시, 칸에 배정.
                            string temp = tbox.Text;
                            //values[i, j] = temp;
                            if (!Assign(values, i, j, temp))
                            {
                                MessageBox.Show(i.ToString() + " " + j.ToString() + " " + temp);
                                return;//(i,j)local_values에 temp를 배정할수없는경우.
                            }
                            //숫자d를 칸에 배정 할때. 같은 유닛의 후보들중 숫자d를 제거.
                            //같은 유닛에 어떤숫자가 들어갈수있는칸이 하나밖에 없다면, 거기에 숫자를 쓴다.
                        }
                    }
                }
            }
            values = search(values);

            display(values);

            sw.Stop();
            textBox2.Text = sw.ElapsedMilliseconds.ToString() + "ms";


        }
        //clear
        private void button11_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < 9; j++)
                {
                    //컨트롤 이름 a1~i9 찾는다.
                    Control[] ctrls = this.Controls.Find(Convert.ToChar('a' + i).ToString() + (j + 1).ToString(), false);
                    if (ctrls != null && ctrls.Length > 0)
                    {
                        TextBox tbox = ctrls[0] as TextBox;
                        tbox.Text = "";
                    }
                }
            }
        }

        //apply
        private void button12_Click(object sender, EventArgs e)
        {
            string str = textBox1.Text;
            int i = 0;
            int j = 0;
            foreach (char ch in str)
            {

                if (ch == '\n' || ch == '\0') continue;
                if (ch == '0' || ch == '.')
                {
                    j++;
                    if (j >= 9)
                    {
                        i++;
                        j = 0;
                    }
                    continue;
                }
                Control[] ctrls = this.Controls.Find(Convert.ToChar('a' + i).ToString() + (j + 1).ToString(), false);
                if (ctrls != null && ctrls.Length > 0)
                {
                    TextBox tbox = ctrls[0] as TextBox;

                    tbox.Text = ch.ToString();
                }
                j++;
                if (j >= 9)
                {
                    i++;
                    j = 0;
                }
            }

        }


    }
}
