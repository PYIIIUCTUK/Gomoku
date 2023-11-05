using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Gomoku
{
    public partial class Game : Form
    {
        Menu menu;

        int S, W, H;
        int offsetX;
        int offsetY;

        int mouseX;
        int mouseY;

        Player curPlayer;

        List<Player> players = new List<Player>();
        List<List<int>> map = new List<List<int>>();

        public Game(Menu myMenu)
        {
            InitializeComponent();

            menu = myMenu;
        }

        private void Game_Shown(object sender, EventArgs e)
        {
            WindowState = FormWindowState.Maximized;
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.Sizable;

            H = 15;
            W = 15;

            S = ClientSize.Height / H;
            offsetX = (ClientSize.Width - S * W) / 2;
            offsetY = (ClientSize.Height % S) / 2;

            for (int i = 0; i < H; i++)
            {
                List<int> line = new List<int>();
                for (int j = 0; j < W; j++)
                {
                    line.Add(0);
                }
                map.Add(line);
            }
            map[H / 2][W / 2] = 2;

            players.Add(new Player(1, Brushes.DarkOrange, Pens.White));
            players.Add(new Player(2, Brushes.Black, Pens.Black));

            curPlayer = players[0];
        }
        private void Game_FormClosed(object sender, FormClosedEventArgs e)
        {
            menu.Show();
        }

        private void Game_Paint(object sender, PaintEventArgs e)
        {
            Pen pen = new Pen(Brushes.Black, 3);
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    e.Graphics.DrawLine(pen, j * S + S / 2 + offsetX, i * S + offsetY,
                        j * S + S / 2 + offsetX, i * S + S + offsetY);
                    e.Graphics.DrawLine(pen, j * S + offsetX, i * S + S / 2 + offsetY,
                        j * S + S + offsetX, i * S + S / 2 + offsetY);

                    if (map[i][j] != 0)
                    {
                        e.Graphics.FillEllipse(players[map[i][j] - 1].Brush, j * S + offsetX,
                                                                i * S + offsetY, S, S);
                    }
                }
            }
            e.Graphics.DrawRectangle(pen, offsetX, offsetY, S * W, S * H);

            if (curPlayer.Ind == 1)
            {
                e.Graphics.DrawString($"Ход Белых", new Font("Times New Roman", 36, FontStyle.Bold),
                      Brushes.Black, new PointF(W * S + S / 2 + offsetX, S / 2 + S * 4 + offsetY));
            }
            else
            {
                e.Graphics.DrawString($"Ход Черных", new Font("Times New Roman", 36, FontStyle.Bold),
                   Brushes.Black, new PointF(W * S + S / 2 + offsetX, S / 2 + S * 4 + offsetY));
            }

            e.Graphics.FillEllipse(players[curPlayer.Ind - 1].Brush, mouseX * S + offsetX + 5,
                                                                mouseY * S + offsetY + 5, S - 10, S - 10);
        }

        private void Game_MouseMove(object sender, MouseEventArgs e)
        {
            int x = (e.X - offsetX);
            int y = (e.Y - offsetY);
            if (x < 0 || x >= W * S || y < 0 || y >= H * S) { return; }
            x /= S;
            y /= S;

            if (x != mouseX || y != mouseY)
            {
                mouseX = x;
                mouseY = y;
                Invalidate();
            }
        }
        private void Game_MouseClick(object sender, MouseEventArgs e)
        {
            int x = (e.X - offsetX);
            int y = (e.Y - offsetY);
            if (x < 0 || x >= W * S || y < 0 || y >= H * S) { return; }

            x /= S;
            y /= S;
            if (map[y][x] == 0)
            {
                map[y][x] = curPlayer.Ind;
                if (CheckWin(x, y))
                {
                    if (curPlayer.Ind == 1)
                    {
                        MessageBox.Show("White Win");
                    }
                    else
                    {
                        MessageBox.Show("Black Win");
                    }
                    Close();
                    return;
                }
                ChangeTurn();

                Invalidate();
            }
        }

        private bool CheckDraw()
        {
            for (int i = 0; i < map.Count; i++)
            {
                for (int j = 0; j < map[i].Count; j++)
                {
                    if (map[i][j] == 0) { return false; }
                }
            }
            return true;
        }
        private void ChangeTurn()
        {
            if (CheckDraw())
            {
                MessageBox.Show("Draw");
                Close();
                return;
            }
            if (curPlayer.Ind == players[0].Ind)
            {
                curPlayer = players[1];
            }
            else { curPlayer = players[0]; }
        }
        private bool CheckWin(int x, int y)
        {
            int res = 1;
            for (int j = 1; j <= 5; j++)
            {
                if (x - j < 0 || x - j >= W) { break; }
                if (map[y][x - j] == curPlayer.Ind)
                {
                    res++;
                }
                else { break; }
            }
            for (int j = 1; j <= 5; j++)
            {
                if (x + j < 0 || x + j >= W) { break; }
                if (map[y][x + j] == curPlayer.Ind)
                {
                    res++;
                }
                else { break; }
            }
            if (res == 5) { return true; }
            else { res = 1; }

            for (int i = 1; i <= 5; i++)
            {
                if (y - i < 0 || y - i >= H) { break; }
                if (map[y - i][x] == curPlayer.Ind)
                {
                    res++;
                }
                else { break; }
            }
            for (int i = 1; i <= 5; i++)
            {
                if (y + i < 0 || y + i >= H) { break; }
                if (map[y + i][x] == curPlayer.Ind)
                {
                    res++;
                }
                else { break; }
            }
            if (res == 5) { return true; }
            else { res = 1; }

            for (int i = 1; i <= 5; i++)
            {
                if (x - i < 0 || x - i >= W || y - i < 0 || y - i >= H) { break; }
                if (map[y - i][x - i] == curPlayer.Ind)
                {
                    res++;
                }
                else { break; }
            }
            for (int i = 1; i <= 5; i++)
            {
                if (x + i < 0 || x + i >= W || y + i < 0 || y + i >= H) { break; }
                if (map[y + i][x + i] == curPlayer.Ind)
                {
                    res++;
                }
                else { break; }
            }
            if (res == 5) { return true; }
            else { res = 1; }

            for (int i = 1; i <= 5; i++)
            {
                if (x + i < 0 || x + i >= W || y - i < 0 || y - i >= H) { break; }
                if (map[y - i][x + i] == curPlayer.Ind)
                {
                    res++;
                }
                else { break; }
            }
            for (int i = 1; i <= 5; i++)
            {
                if (x - i < 0 || x - i >= W || y + i < 0 || y + i >= H) { break; }
                if (map[y + i][x - i] == curPlayer.Ind)
                {
                    res++;
                }
                else { break; }
            }
            if (res == 5) { return true; }

            return false;
        }
    }
}
