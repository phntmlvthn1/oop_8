using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laba4OOP
{
	public partial class Form1 : Form
	{
		//хранилище объектов
		Storage storage = new Storage(40);
		List<Group> groupList = new List<Group>();

		int numberOfObjects = 0;
		int numberOfEveryObject = 0;
		public Form1()
		{
			InitializeComponent();
		}

		private void Form1_MouseDown(object sender, MouseEventArgs e)
		{
			for (int i = 0; i < storage.GetCount(); i++)
			{
				//проверка на нажатие по фигуре
				if (storage.HaveObject(i))
				{
					if (storage.GetObject(i).CheckClickOnObject(e.X, e.Y))
					{
						return;
					}
				}
			}
			if (rbCircle.Checked)
				storage.SetObject(numberOfObjects, new CCircle(), numberOfEveryObject);
			if (rbSquare.Checked)
				storage.SetObject(numberOfObjects, new CSquare(), numberOfEveryObject);
			storage.GetObject(numberOfObjects).SetCoords(e.X, e.Y);
			numberOfObjects++;
			numberOfEveryObject++;
			Invalidate();
			storage.observers.Invoke(this, null);
		}
		public void UpdateFromStorage(object sender, EventArgs e)
		{
			treeView1.Nodes.Clear();

			for (int i = 0; i < storage.GetCount(); i++)
			{
				TreeNode n = new TreeNode();
				if (storage.getObjectFromindex(i) != null)
				{
					treeView1.Nodes.Add(ObjectToNode(storage.getObjectFromindex(i), n, i));
				}
			}
			Invalidate();
		}

		TreeNode ObjectToNode(Object obj, TreeNode t, int i)
		{
			int k = 0;
			if (obj.chosen)
				t.Checked = true;
			TreeNode n;
			if (obj is Group)
			{
				t.Text = i + " " + obj.Otostring();
				if (obj.chosen)
					t.Checked = true;

				for (int j = 0; j < obj.GetLengthOfGroup(); j++)
				{
					if (obj.GetObject(j) != null)
					{
						n = new TreeNode();
						k = t.Nodes.Add(ObjectToNode(obj.GetObject(j), n, i * 100 + k));
					}
					k++;
				}
			}
			else
			{
				t.Text = i + " " + obj.Otostring();
			}
			return t;
		}
		private void Form1_Paint(object sender, PaintEventArgs e)
		{

			for (int i = 0; i < storage.GetCount(); i++)
			{
				if (storage.HaveObject(i))
				{
					if (storage.GetObject(i).CheckChosen())
						storage.GetObject(i).DrawRedObject();
					else if (storage.GetObject(i).colored)
						storage.GetObject(i).DrawGreenObject();
					else
					{
						storage.GetObject(i).DrawObject();
					}
				}
			}
		}

		private void Form1_MouseUp(object sender, MouseEventArgs e)
		{
			for (int i = 0; i < storage.GetCount(); i++)
			{
				if (storage.HaveObject(i))
				{
					if (Control.ModifierKeys == Keys.Control && e.Button == System.Windows.Forms.MouseButtons.Left)
					{
						storage.GetObject(i).CheckChangeChosen(e.X, e.Y);
						Invalidate();
					}
					else if (e.Button == System.Windows.Forms.MouseButtons.Left)
					{
						storage.GetObject(i).CheckChangeUnchosen();
						storage.GetObject(i).CheckChangeChosen(e.X, e.Y);
					}
					Invalidate();
				}
			}
			storage.observers.Invoke(this, null);
		}

		private void btnDelete_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < storage.GetCount(); i++)
			{
				if (storage.HaveObject(i))
					if (storage.GetObject(i).CheckChosen())
					{
						storage.GetObject(i).DeleteObject();
						storage.DeleteObject(i);
						Invalidate();
					}
			}
			storage.observers.Invoke(this, null);
		}

		private void Form1_KeyDown(object sender, KeyEventArgs e)
		{
			int object0 = 0;

			for (int i = 0; i < storage.GetCount(); i++)
			{
				if (storage.HaveObject(i))
				{
					if (storage.GetObject(i).CheckChosen())
					{
						object0 = i;
						break;
					}
				}
			}

			if (e.KeyData == Keys.Q)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
					{
						if (i == object0 - 1)
						{
							storage.GetObject(i).chosen = true;
						}
						else
							storage.GetObject(i).CheckChangeUnchosen();
					}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.E)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
					{
						if (i == object0 + 1)
						{
							storage.GetObject(i).chosen = true;
						}
						else
							storage.GetObject(i).CheckChangeUnchosen();
					}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.C)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
						if (storage.GetObject(i).CheckChosen())
						{
							if (rbGreen.Checked)
							{
								storage.GetObject(i).colored = true;
								storage.GetObject(i).CheckChangeUnchosen();
							}
						}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.X)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
						if (storage.GetObject(i).CheckChosen())
						{
							storage.GetObject(i).SizeUpObject();
						}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.Z)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
						if (storage.GetObject(i).CheckChosen())
						{
							storage.GetObject(i).SizeDownObject();
						}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.A)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
					{
						if (storage.GetObject(i).isclayed)
						{
							storage.GetObject(i).LeftObject();
							for (int j = 0; j < storage.GetCount(); j++)
							{
								if (storage.HaveObject(j) && !storage.GetObject(j).isclayed)
									if (storage.GetObject(j).isclayed || (((storage.GetObject(j).xCoord - storage.GetObject(j).size) < storage.GetObject(i).xCoord) && ((storage.GetObject(j).xCoord + storage.GetObject(j).size) > storage.GetObject(i).xCoord) && ((storage.GetObject(j).yCoord - storage.GetObject(i).size - storage.GetObject(i).size) < storage.GetObject(i).yCoord) && ((storage.GetObject(j).yCoord + storage.GetObject(i).size) > storage.GetObject(i).yCoord)))
									{
										storage.GetObject(j).isclayed = true;
									}
							}
						}
						else if (storage.GetObject(i).CheckChosen())
						{
							storage.GetObject(i).LeftObject();
						}
					}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.D)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
					{
						if (storage.GetObject(i).isclayed)
						{
							storage.GetObject(i).RightObject();
							for (int j = 0; j < storage.GetCount(); j++)
							{
								if (storage.HaveObject(j) && !storage.GetObject(j).isclayed)
									if (storage.GetObject(j).isclayed || (((storage.GetObject(j).xCoord - storage.GetObject(j).size) < storage.GetObject(i).xCoord) && ((storage.GetObject(j).xCoord + storage.GetObject(j).size) > storage.GetObject(i).xCoord) && ((storage.GetObject(j).yCoord - storage.GetObject(i).size - storage.GetObject(i).size) < storage.GetObject(i).yCoord) && ((storage.GetObject(j).yCoord + storage.GetObject(i).size) > storage.GetObject(i).yCoord)))
									{
										storage.GetObject(j).isclayed = true;
									}
							}
						}
						else if (storage.GetObject(i).CheckChosen())
						{
							storage.GetObject(i).RightObject();
						}
					}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.W)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
					{
						if (storage.GetObject(i).isclayed)
						{
							storage.GetObject(i).DownObject();
							for (int j = 0; j < storage.GetCount(); j++)
							{
								if (storage.HaveObject(j) && !storage.GetObject(j).isclayed)
									if (storage.GetObject(j).isclayed || (((storage.GetObject(j).xCoord - storage.GetObject(j).size) < storage.GetObject(i).xCoord) && ((storage.GetObject(j).xCoord + storage.GetObject(j).size) > storage.GetObject(i).xCoord) && ((storage.GetObject(j).yCoord - storage.GetObject(i).size - storage.GetObject(i).size) < storage.GetObject(i).yCoord) && ((storage.GetObject(j).yCoord + storage.GetObject(i).size) > storage.GetObject(i).yCoord)))
									{
										storage.GetObject(j).isclayed = true;
									}
							}
						}
						else if (storage.GetObject(i).CheckChosen())
						{
							storage.GetObject(i).DownObject();
						}
					}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.S)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
					{
						if (storage.GetObject(i).isclayed)
						{
							storage.GetObject(i).UpObject();
							for (int j = 0; j < storage.GetCount(); j++)
							{
								if (storage.HaveObject(j) && !storage.GetObject(j).isclayed)
									if (storage.GetObject(j).isclayed || (((storage.GetObject(j).xCoord - storage.GetObject(j).size) < storage.GetObject(i).xCoord) && ((storage.GetObject(j).xCoord + storage.GetObject(j).size) > storage.GetObject(i).xCoord) && ((storage.GetObject(j).yCoord - storage.GetObject(i).size - storage.GetObject(i).size) < storage.GetObject(i).yCoord) && ((storage.GetObject(j).yCoord + storage.GetObject(i).size) > storage.GetObject(i).yCoord)))
									{
										storage.GetObject(j).isclayed = true;
									}
							}
						}
						else if (storage.GetObject(i).CheckChosen())
						{
							storage.GetObject(i).UpObject();
						}
					}
				}
				Invalidate();
			}

			if (e.KeyData == Keys.Delete)
			{
				for (int i = 0; i < storage.GetCount(); i++)
				{
					if (storage.HaveObject(i))
						if (storage.GetObject(i).CheckChosen())
						{
							storage.DeleteObject(i);
						}
				}
				Invalidate();
			}
			storage.observers.Invoke(this, null);
		}

		private void btnGroup_Click(object sender, EventArgs e)
		{
			int chosenObjects = 0;
			for (int i = 0; i < storage.GetCount(); i++)
			{
				if (storage.HaveObject(i) && storage.GetObject(i).CheckChosen())
					chosenObjects++;
			}
			groupList.Add(new Group());
			for (int i = 0; i < storage.GetCount(); i++)
			{
				if (storage.HaveObject(i) && storage.GetObject(i).CheckChosen())
				{
					if (storage.GetObject(i) is Group)
					{
						for (int j = 0; j < groupList[storage.GetObject(i).numberInGroupList].GetLengthOfGroup(); j++)
						{
							groupList[groupList.Count - 1].addObject(groupList[storage.GetObject(i).numberInGroupList].GetObject(j));
							groupList[groupList.Count - 1].numberInGroupList = groupList.Count - 1;

						}
						for (int j = 0; j < groupList[storage.GetObject(i).numberInGroupList].GetLengthOfGroup(); j++)
						{
							groupList[groupList.Count - 1].GetObject(j).numberInGroupList = groupList.Count - 1;
						}
					}
					else
					{
						groupList[groupList.Count - 1].addObject(storage.GetObject(i));
						for (int j = 0; j < groupList[storage.GetObject(i).numberInGroupList].GetLengthOfGroup(); j++)
						{
							groupList[groupList.Count - 1].numberInGroupList = groupList.Count - 1;
							groupList[groupList.Count - 1].GetObject(j).numberInGroupList = groupList.Count - 1;
						}
					}
					storage.DeleteObject(i);
				}
			}
			storage.SetObject(numberOfObjects, groupList[groupList.Count - 1], numberOfEveryObject);
			numberOfObjects++;
			groupList[groupList.Count - 1].chosen = false;
			Invalidate();
			storage.observers.Invoke(this, null);
		}

		private void btnUngroup_Click(object sender, EventArgs e)
		{
			int numberOfGroup = 0;

			for (int i = 0; i < groupList.Count; i++)
			{
				if (groupList[i].CheckChosen())
				{
					numberOfGroup = i;
				}
			}

			for (int i = 0; i < groupList[numberOfGroup].GetLengthOfGroup(); i++)
			{
				storage.SetObject(numberOfObjects, groupList[numberOfGroup].GetObject(i), groupList[numberOfGroup].GetObject(i).numberOfObject);
				numberOfObjects++;
			}
			groupList[numberOfGroup].DeleteGroup();
			Invalidate();
			storage.observers.Invoke(this, null);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			storage.SaveObjs();
		}

		private void btnLoad_Click(object sender, EventArgs e)
		{
			storage.LoadObjs();
			System.Drawing.Graphics MyFormPaint = this.CreateGraphics();
			Color col = Color.White;
			MyFormPaint.Clear(col); // очистка поверхности и заливка ее белым цветом
			Invalidate();
			storage.observers.Invoke(this, null);
		}

		private void Form1_Load(object sender, EventArgs e)
		{
			storage.observers = new System.EventHandler(this.UpdateFromStorage);
			//treeView1.Hide();
		}

		private void treeView1_Click(object sender, EventArgs e)
		{
			foreach (TreeNode n in treeView1.Nodes)
			{
				//recursiveTotalNodes++;
				if (n.Checked)
				{
					int j = 0;
					string s = n.Text;
					string x = "";
					string y = "";
					while (s[j] != ',')
					{
						j++;
					}
					j++;
					while (s[j] != ',')
					{
						x += s[j];
						j++;
					}
					j++;
					while (s[j] != ')')
					{
						y += s[j];
						j++;
					}
					storage.SelectByCoords(Int32.Parse(x), Int32.Parse(y));
				}
				else
				{
					int j = 0;
					string s = n.Text;
					string x = "";
					string y = "";
					while (s[j] != ',')
					{
						j++;
					}
					j++;
					while (s[j] != ',')
					{
						x += s[j];
						j++;
					}
					j++;
					while (s[j] != ')')
					{
						y += s[j];
						j++;
					}
					storage.UnselectByCoords(Int32.Parse(x), Int32.Parse(y));
				}

			}
			System.Drawing.Graphics MyFormPaint = this.CreateGraphics();
			Color col = Color.White;
			MyFormPaint.Clear(col); // очистка поверхности и заливка ее белым цветом
			Invalidate();
		}

		private void treeView1_MouseMove(object sender, MouseEventArgs e)
		{
			//Invalidate();
		}

		private void btnLip_Click(object sender, EventArgs e)
		{
			for (int i = 0; i < storage.GetCount(); i++)
			{
				if (storage.HaveObject(i) && storage.GetObject(i).chosen)
				{
					storage.GetObject(i).isclayed = true;
					storage.GetObject(i).Selectclayed(storage.GetObject(i));
				}
			}
		}

		private void rbGreen_CheckedChanged(object sender, EventArgs e)
		{

		}
	}
	class Clayed : Object
	{
		public Clayed()
		{
			isclayed = true;
		}
		override public void Save(StreamWriter _file) //сохранение объекта в файл
		{
			_file.WriteLine("L"); //пишем, что записываемый объект - липкий
			clayed.Save(_file); //сохраняем его
		}
		override public void Load(StreamReader _file) //выгрузка данных об объекте из файла
		{
			Factory factory = new Factory(); //factory для создания объектов
			char code;  //код, определяюший тип объекта
			code = Convert.ToChar(_file.ReadLine()); //считываем тип объекта
			clayed = factory.CreateObject(code); //factory создает объект определенного типа
			if (clayed != null)
			{
				clayed.Load(_file); //считываем информацию о объекте из файла
			}
		}
		override public string Otostring()
		{
			return "Clayed" + clayed.Otostring();
		}
		public override void UpObject()
		{
			if (yCoord >= Form.ActiveForm.ClientSize.Height - size - 2)
				yCoord = Form.ActiveForm.ClientSize.Height - size - 2;
			yCoord++;
		}

		public override void DownObject()
		{
			if (yCoord <= 1)
				yCoord = 1;
			yCoord--;
		}

		public override void RightObject()
		{
			if (xCoord >= Form.ActiveForm.ClientSize.Width - size - 2)
				xCoord = Form.ActiveForm.ClientSize.Width - size - 2;
			xCoord++;
		}

		public override void LeftObject()
		{
			if (xCoord <= 1)
				xCoord = 1;
			xCoord--;
		}

		public override void DrawObject()
		{
			//рисование круга
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Black);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle ellipse = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawEllipse(myPen, ellipse);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}

		public override void DrawRedObject()
		{
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle ellipse = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawEllipse(myPen, ellipse);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}

		public override void DrawGreenObject()
		{
			color = Color.Green;
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Green);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle ellipse = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawEllipse(myPen, ellipse);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}

		public override void DeleteObject()
		{
			chosen = false;
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.White);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle ellipse = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawEllipse(myPen, ellipse);
			DeleteNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}
	}
	class Storage
	{
		private int size;
		Object[] storage;
		public System.EventHandler observers;

		Storage()
		{
			size = 0;
		}

		//конструктор
		public Storage(int size)
		{
			this.size = size;
			storage = new Object[size];
		}
		public Object getObjectFromindex(int i)
		{
			if (storage[i] != null)
				return storage[i];
			return null;
		}
		public void SelectByCoords(int x, int y)
		{
			for (int i = 0; i < size; i++)
			{
				if (storage[i] != null)
					if ((storage[i].xCoord == x) && (storage[i].yCoord == y))
						storage[i].chosen = true;
			}
		}
		public void UnselectByCoords(int x, int y)
		{
			for (int i = 0; i < size; i++)
			{
				if (storage[i] != null)
					if ((storage[i].xCoord == x) && (storage[i].yCoord == y))
						storage[i].chosen = false;
			}
		}

		//добавить объект к хранилище
		public void SetObject(int i, Object newObject, int numberOfEveryObject)
		{
			storage[i] = newObject;
			storage[i].numberOfObject = numberOfEveryObject;
			for (int j = 0; j < storage.Length; j++)
			{
				if (storage[j] != null)
					storage[j].chosen = false;
			}
			storage[i].chosen = true;
		}

		//ссылка на объект хранилище
		public Object GetObject(int i)
		{
			return storage[i];
		}

		//количество объектов в хранилище
		public int GetCount()
		{
			return size;
		}

		//удалить объект из хранилища 
		public void DeleteObject(int i)
		{
			storage[i] = null;
		}

		//полностью удалить объект
		public void DestroyObject(int i)
		{
			if (size != 0)
			{
				DeleteObject(i);
			}
		}

		//проверка на наличие объекта в хранилище
		public bool HaveObject(int i)
		{
			if (storage[i] != null)
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		public void SaveObjs() //функция сохранения хранилища в файл
		{

			string path = @"C:\Users\1234\Desktop\OOPLaba4\save.txt"; //путь до файла
			StreamWriter cfile = new StreamWriter(path, false); //создаем записыватель файла
			cfile.WriteLine(size); //записываем размер хранилища
			for (int i = 0; i < size; i++)
			{
				if (storage[i] != null) //если объект существует
				{
					{
						storage[i].Save(cfile); //сохраняем его
					}
				}
			}
			cfile.Close();
		}
		public void LoadObjs() //выгрузка объектов из файла в хранилище
		{
			string path = @"C:\Users\1234\Desktop\OOPLaba4\save.txt"; ; //путь до файла
			Factory factory = new Factory(); //factory для создания объектов
			StreamReader sr = new StreamReader(path); //читатель файла
			char code;  //код, определяюший тип объекта
			size = Convert.ToInt32(sr.ReadLine());
			storage = new Object[100]; //создаем хранилище определенного размера
			for (int i = 0; i < size; i++)
			{
				if (!sr.EndOfStream)
				{
					code = Convert.ToChar(sr.ReadLine()); //считываем тип объекта
					storage[i] = factory.CreateObject(code); //factory создает объект определенного типа
					if (storage[i] != null)
					{
						storage[i].Load(sr); //считываем информацию о объекте из файла
					}
				}
			}
			sr.Close(); //закрываем файл
		}
	}

	//класс группировки для паттерна Composite
	/// <remarks></remarks>
	class Group : Object
	{
		//Color color = new Color();
		static Random rand = new Random();
		int random1 = rand.Next(256);
		int random2 = rand.Next(256);
		int random3 = rand.Next(256);

		public Group()
		{
			color = Color.FromArgb(random1, random2, random3);
		}

		private List<Object> groupObj = new List<Object>();

		public override string Otostring()
		{
			return "Group(" + groupObj.Count + "," + xCoord + "," + yCoord + ") ";
		}

		public override int GetLengthOfGroup()
		{
			return groupObj.Count;
		}

		public void DeleteObject(Object obj)
		{
			groupObj.Remove(obj);
		}

		public void addObject(Object obj)
		{
			numberInGroupList = obj.numberInGroupList;
			groupObj.Add(obj);
			if (groupObj.Count == 1)
			{
				xCoord = obj.xCoord;
				yCoord = obj.yCoord;
			}
			obj.chosen = false;
			foreach (Object obj0 in groupObj)
			{
				obj0.color = color;
			}
		}

		//рисование круга
		public override void DrawObject()
		{
			foreach (Object obj in groupObj)
			{
				System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.FromArgb(color.ToArgb()));
				System.Drawing.Graphics formGraphics;
				formGraphics = Form.ActiveForm.CreateGraphics();
				if (obj is CCircle)
				{
					Rectangle ellipse = new Rectangle(obj.xCoord, obj.yCoord, obj.size, obj.size);
					formGraphics.DrawEllipse(myPen, ellipse);
				}
				else
				{
					Rectangle rtg = new Rectangle(obj.xCoord, obj.yCoord, obj.size, obj.size);
					formGraphics.DrawRectangle(myPen, rtg);
				}
				obj.DrawNumber(obj.numberOfObject);

				myPen.Dispose();
				formGraphics.Dispose();
			}
		}

		public override void DrawRedObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.DrawRedObject();
			}
		}

		public override void DrawGreenObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.DrawGreenObject();
			}
		}

		public override bool CheckClickOnObject(int x, int y)
		{
			foreach (Object obj in groupObj)
			{
				if (obj.CheckClickOnObject(x, y))
					return true;
			}
			return false;
		}

		public override void CheckChangeChosen(int x, int y)
		{
			foreach (Object obj in groupObj)
			{
				if (obj.CheckClickOnObject(x, y))
				{
					chosen = true;
					foreach (Object obj2 in groupObj)
					{
						obj2.chosen = true;
					}
					return;
				}
				chosen = false;
			}
		}

		public override bool CheckChosen()
		{
			foreach (Object obj in groupObj)
			{
				if (obj.CheckChosen())
					return true;
			}
			return false;
		}

		public override void CheckChangeUnchosen()
		{
			foreach (Object obj in groupObj)
			{
				obj.CheckChangeUnchosen();
			}
		}

		public override void UpObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.UpObject();
			}
		}

		public override void DownObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.DownObject();
			}
		}

		public override void RightObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.RightObject();
			}
		}

		public override void LeftObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.LeftObject();
			}
		}
		public override void SizeUpObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.SizeUpObject();
			}
		}

		public override Object GetObject(int i)
		{
			return groupObj[i];
		}

		public override void SizeDownObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.SizeDownObject();
			}
		}

		public void DeleteGroup()
		{
			groupObj.Clear();
		}

		//удаление объекта из формы
		public override void DeleteObject()
		{
			foreach (Object obj in groupObj)
			{
				obj.DeleteObject();
			}
			DeleteGroup();
		}
		public override void Save(StreamWriter _file) //сохранение объекта
		{
			_file.WriteLine("G"); //пишем, что записываемый объект - группа
			_file.WriteLine(groupObj.Count); //записываем размер группы
			_file.WriteLine(random1); //записываем rand1 группы
			_file.WriteLine(random2); //записываем rand2 группы
			_file.WriteLine(random3); //записываем rand3 группы
			foreach (Object obj in groupObj)
			{
				if (obj != null) //если объект существует
				{
					{
						obj.Save(_file); //сохраняем его
					}
				}
			}
		}
		public override void Load(StreamReader _file)
		{
			Factory factory = new Factory(); //factory для создания объектов
			char code;  //код, определяюший тип объекта
			size = Convert.ToInt32(_file.ReadLine());
			random1 = Convert.ToInt32(_file.ReadLine());
			random2 = Convert.ToInt32(_file.ReadLine());
			random3 = Convert.ToInt32(_file.ReadLine());
			groupObj = new List<Object>(); //создаем хранилище

			for (int i = 0; i < size; i++)
			{
				code = Convert.ToChar(_file.ReadLine()); //считываем тип объекта
				groupObj.Add(factory.CreateObject(code));
				if (groupObj[i] != null)
				{
					groupObj[i].Load(_file); //считываем информацию о объекте из файла
				}
			}
		}
	}

	//базовый класс
	class Object
	{
		public int xCoord;
		public int yCoord;
		public int size = 30;
		public int numberInGroupList;

		//метка выделенности
		public bool chosen = false;
		public bool isclayed = false;
		public Color color = Color.Black;

		//метка окрашенности
		public bool colored = false;
		public void Selectclayed(Object a)
		{
			clayed = a;
			xCoord = clayed.xCoord;
			yCoord = clayed.yCoord;
		}
		public Object clayed = null;

		//номер объекта
		public int numberOfObject = 0;
		public virtual int GetLengthOfGroup()
		{
			return 0;
		}

		//задать координаты
		public void SetCoords(int xCoord, int yCoord)
		{
			this.xCoord = xCoord;
			this.yCoord = yCoord;
		}

		//нарисовать черный объект
		public virtual void DrawObject()
		{
			Console.WriteLine("Object");
		}

		//нарисовать красный объект
		public virtual void DrawRedObject()
		{
			Console.WriteLine("Object");
		}
		public virtual void DrawGreenObject()
		{
			Console.WriteLine("Object");
		}

		public virtual bool CheckChosen()
		{
			if (chosen)
				return true;
			else
				return false;
		}

		//проверка на нажатие на объекта
		public virtual bool CheckClickOnObject(int x, int y)
		{
			if (((x - size) < xCoord) && (x + size > xCoord) && ((y - size - size) < yCoord) && (y + size > yCoord))
				return true;
			else
				return false;
		}

		public virtual void CheckChangeChosen(int x, int y)
		{
			if (CheckClickOnObject(x, y))
				chosen = true;
		}

		public virtual Object GetObject(int i)
		{
			return new Object();
		}

		public virtual void CheckChangeUnchosen()
		{
			chosen = false;
		}

		//удаление объекта из формы
		public virtual void DeleteObject()
		{
			Console.WriteLine("Object");
		}

		//рисование номера объекта
		public void DrawNumber(int numberOfCircle)
		{
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			string drawString = numberOfCircle.ToString();
			System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 14);
			System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.Black);
			System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
			formGraphics.DrawString(drawString, drawFont, drawBrush, xCoord - size / 2, yCoord - size / 2, drawFormat);
			drawFont.Dispose();
			drawBrush.Dispose();
			formGraphics.Dispose();
		}

		//удаление номера объекта
		public void DeleteNumber(int numberOfCircle)
		{
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			string drawString = numberOfCircle.ToString();
			System.Drawing.Font drawFont = new System.Drawing.Font("Arial", 14);
			System.Drawing.SolidBrush drawBrush = new System.Drawing.SolidBrush(System.Drawing.Color.White);
			System.Drawing.StringFormat drawFormat = new System.Drawing.StringFormat();
			formGraphics.DrawString(drawString, drawFont, drawBrush, xCoord - size / 2, yCoord - size / 2, drawFormat);
			drawFont.Dispose();
			drawBrush.Dispose();
			formGraphics.Dispose();
		}

		public virtual void UpObject()
		{
			if (yCoord >= Form.ActiveForm.ClientSize.Height - size - 2)
				yCoord = Form.ActiveForm.ClientSize.Height - size - 2;
			yCoord++;
		}

		public virtual void DownObject()
		{
			if (yCoord <= 1)
				yCoord = 1;
			yCoord--;
		}

		public virtual void RightObject()
		{
			if (xCoord >= Form.ActiveForm.ClientSize.Width - size - 2)
				xCoord = Form.ActiveForm.ClientSize.Width - size - 2;
			xCoord++;
		}

		public virtual void LeftObject()
		{
			if (xCoord <= 1)
				xCoord = 1;
			xCoord--;
		}
		virtual public string Otostring()
		{
			return "";
		}

		public virtual void SizeUpObject()
		{
			size++;
		}

		public virtual void SizeDownObject()
		{
			size--;
		}

		virtual public void Save(StreamWriter _file) //сохранение объекта в файл
		{

		}

		virtual public void Load(StreamReader _file) //выгрузка данных об объекте из файла
		{

		}
	}

	class CCircle : Object
	{
		public override void DrawObject()
		{
			//рисование круга
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Black);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle ellipse = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawEllipse(myPen, ellipse);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}
		public override string Otostring()
		{
			return "Circle(" + color + "," + xCoord + "," + yCoord + ")";
		}

		public override void DrawRedObject()
		{
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle ellipse = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawEllipse(myPen, ellipse);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}

		public override void DrawGreenObject()
		{
			color = Color.Green;
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Green);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle ellipse = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawEllipse(myPen, ellipse);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}

		public override void DeleteObject()
		{
			chosen = false;
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.White);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle ellipse = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawEllipse(myPen, ellipse);
			DeleteNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}

		public override void Save(StreamWriter _file) //сохранение объекта
		{
			_file.WriteLine("C"); //пишем, что записываемый объект - круг
			_file.WriteLine(xCoord); //записываем его данные (координаты,радиус и цвет)
			_file.WriteLine(yCoord);
			_file.WriteLine(size);
			_file.WriteLine(numberOfObject);
			_file.WriteLine(colored);
		}
		public override void Load(StreamReader _file)
		{
			xCoord = Convert.ToInt32(_file.ReadLine());
			yCoord = Convert.ToInt32(_file.ReadLine());
			size = Convert.ToInt32(_file.ReadLine());
			numberOfObject = Convert.ToInt32(_file.ReadLine());
			colored = Convert.ToBoolean(_file.ReadLine());
		}
	}

	class CSquare : Object
	{

		public override void DrawObject()
		{
			//рисование квадрата
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Black);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle rtg = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawRectangle(myPen, rtg);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}
		public override string Otostring()
		{
			return "Square(" + color + "," + xCoord + "," + yCoord + ")"; ;
		}

		public override void DrawRedObject()
		{
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Red);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle rtg = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawRectangle(myPen, rtg);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}
		public override void DrawGreenObject()
		{
			color = Color.Green;
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.Green);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle rtg = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawRectangle(myPen, rtg);
			DrawNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}

		public override void DeleteObject()
		{
			chosen = false;
			System.Drawing.Pen myPen = new System.Drawing.Pen(System.Drawing.Color.White);
			System.Drawing.Graphics formGraphics;
			formGraphics = Form.ActiveForm.CreateGraphics();
			Rectangle rtg = new Rectangle(xCoord, yCoord, size, size);
			formGraphics.DrawRectangle(myPen, rtg);
			DeleteNumber(this.numberOfObject);

			myPen.Dispose();
			formGraphics.Dispose();
		}

		public override void Save(StreamWriter _file) //сохранение объекта
		{
			_file.WriteLine("R"); //пишем, что записываемый объект - квадрат
			_file.WriteLine(xCoord); //записываем его данные
			_file.WriteLine(yCoord);
			_file.WriteLine(size);
			_file.WriteLine(numberOfObject);
			_file.WriteLine(colored);
		}
		public override void Load(StreamReader _file)
		{
			xCoord = Convert.ToInt32(_file.ReadLine());
			yCoord = Convert.ToInt32(_file.ReadLine());
			size = Convert.ToInt32(_file.ReadLine());
			numberOfObject = Convert.ToInt32(_file.ReadLine());
			colored = Convert.ToBoolean(_file.ReadLine());

		}
	}

	class Factory
	{
		public Object CreateObject(char code)
		{
			Object obj = null;
			switch (code)
			{
				case 'C':
					obj = new CCircle();
					break;
				case 'R':
					obj = new CSquare();
					break;
				case 'G':
					obj = new Group();
					break;
				case 'L':
					obj = new Clayed();
					break;
				default:
					break;

			}
			return obj;
		}
	}
}
