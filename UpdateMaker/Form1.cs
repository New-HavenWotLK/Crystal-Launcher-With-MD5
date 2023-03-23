﻿//
// Copyright (c) 2010-2012, MatthiWare
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software. 
// You shall include 'MatthiWare' in the credits/about section of the program
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
// DEALINGS IN THE SOFTWARE.
//

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using CrystalUpdater.Data;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
namespace UpdateMaker
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnRemove_Click(object sender, EventArgs e)
        {
            listView1.SelectedItems[0].Remove();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        { 
            string[] files = Directory.GetFiles(@"./Files/", "*", SearchOption.AllDirectories);
           
            foreach (var file in files)
            {
                FileInfo fi = new FileInfo(file);
                var size = fi.Length;
                string fullPath = fi.FullName.Split(new string[] { "Files" }, StringSplitOptions.None)[1];
                string finalPath = fullPath.Split(new string[] { fi.Name }, StringSplitOptions.None)[0];
                string checksumMd5 = GetHashFromFile(file);
                ListViewItem lvItem = new ListViewItem(new String[] { fi.Name, checksumMd5.ToString(), finalPath });
                listView1.Items.Add(lvItem);
            }
           
        }

        public static string GetHashFromFile(string fileName)
        {
            int buffersize = 65536;
            using (var md5 = MD5.Create())
            {
                using (var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read, buffersize))
                {
                    var block = new byte[buffersize];
                    int length = 0;
                    Int64 filesize = stream.Length;
                    Int64 bytesread = 0;
                    length = stream.Read(block, 0, buffersize);
                    bytesread += length;
                    while (length == block.Length)
                    {
                        md5.TransformBlock(block, 0, length, null, 0);
                        length = stream.Read(block, 0, buffersize);
                        bytesread += length;
                    }
                    md5.TransformFinalBlock(block, 0, length);
                    bytesread += length;
                }
                var hash = md5.Hash;
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
       


        private void btnGenerate_Click(object sender, EventArgs e)
        {
            UpdateSaveFile file = new UpdateSaveFile(txtVersion.Text);
            foreach (ListViewItem item in listView1.Items)
            {
                file.UpdateFileCollection.Add(new UpdateFileInfo(item.SubItems[0].Text, item.SubItems[1].Text, item.SubItems[2].Text));
            }
            BinaryFormatter bf = new BinaryFormatter();
            FileStream bfStream = File.Open(@".\UpdateInfo.dat", FileMode.OpenOrCreate, FileAccess.ReadWrite);
            bf.Serialize(bfStream, file);
            bfStream.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
