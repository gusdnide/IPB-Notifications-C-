using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FPS_GH
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        class FPSGH
        {
            public static string lPageInicial = "http://forum.fpsgh.org/";
            public static string lNotificacoes = "http://forum.fpsgh.org/index.php?app=core&module=usercp&area=notificationlog";
            public static string iNotificacoes = "ipsHasNotifications";
            public static string lInbox = "http://forum.fpsgh.org/index.php?app=members&module=messaging";
            public static string iInPage = "user_link";
            public static string lLogout = "http://forum.fpsgh.org/index.php?app=core&module=global&section=login&do=logout";
        }
        NotifyIcon nIcon = new NotifyIcon();
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Hide();
            webBrowser1.Navigate(FPSGH.lPageInicial);
            webBrowser1.ScriptErrorsSuppressed = true;
            timer1.Start();
            nIcon.Icon = this.Icon;
            nIcon.Text = "FPSGH Notify";
            nIcon.Visible = true;
            nIcon.BalloonTipClicked += NIcon_BalloonTipClicked;
            timer1.Start();
        }

        private void NIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(FPSGH.lNotificacoes);
        }

        bool Logando = true;
        int UltimoNumNotificacoes = 0;
        int UltimoNumMsg = 0;
        private void sairToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Show();
            webBrowser1.Navigate(FPSGH.lLogout);
        }

        HtmlElement BuscarNotfy()
        {
            HtmlElement Retorno = null;
            foreach (HtmlElement e in webBrowser1.Document.GetElementById("notify_link").All)
            {
                if (e.GetAttribute("className") == FPSGH.iNotificacoes)
                    Retorno = e;
            }
            return Retorno;
        }
        HtmlElement BuscarInbox()
        {
            HtmlElement Retorno = null;
            foreach (HtmlElement e in webBrowser1.Document.GetElementById("inbox_link").All)
            {
                if (e.GetAttribute("className") == FPSGH.iNotificacoes)
                    Retorno = e;
            }
            return Retorno;
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            if (!Logando)
            {
                if (webBrowser1.Url.ToString() == FPSGH.lPageInicial)
                {
                    if (webBrowser1.DocumentText.Contains(FPSGH.iInPage))
                    {
                        if (webBrowser1.DocumentText.Contains(FPSGH.iNotificacoes))
                        {
                            HtmlElement elNotfity = BuscarNotfy();
                            HtmlElement elInbox = BuscarInbox();
                            string MSG = "";
                            if (elNotfity == null && elInbox == null)
                            {
                                webBrowser1.Refresh();
                                return;
                            }
                            if (elNotfity != null)
                            {
                                int Notificacoes = int.Parse(elNotfity.InnerText.Replace(" ", ""));
                                if (Notificacoes != UltimoNumNotificacoes)
                                {
                                    UltimoNumNotificacoes = Notificacoes;
                                    MSG = "Você tem " + Notificacoes + " notificaçoes novas!";
                                }
                            }
                            if(elInbox != null)
                            {
                                int Messages = int.Parse(elInbox.InnerHtml.Replace(" ", ""));
                                if(Messages != UltimoNumMsg)
                                {
                                    UltimoNumMsg = Messages;
                                    MSG += "\n Você tem " + Messages + " Mensagens novas!";
                                }
                            }
                            if(MSG != "")
                            nIcon.ShowBalloonTip(2000, "Novas Notificaçoes", MSG,  ToolTipIcon.Info);
                        }
                        else
                        {
                            webBrowser1.Refresh();
                        }
                    }
                    else
                    {
                        Logando = true;
                        nIcon.Visible = false;
                        this.Show();
                        webBrowser1.Navigate(FPSGH.lPageInicial);
                        MessageBox.Show("Você não está logado!");
                    }
                }
                else
                {
                    webBrowser1.Navigate(FPSGH.lPageInicial);
                }
                webBrowser1.Refresh();
            }

        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            if (webBrowser1.DocumentText.Contains(FPSGH.iInPage)) { this.Hide(); nIcon.Visible = true; Logando = false; }
        }
    }
}
