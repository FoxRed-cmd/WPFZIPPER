   M S B A M L   `   `   `  ���� G  CWPFInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WPFInterfaceWPFInterface      WPFInterface.MainWindow   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:WPFInterface     -     � Title$  Молния��$	��320��$	��350��$	��420��$	��400�����	#FF4D5AAC � 	Resources    FileContext m� Icon��%\  &	        �� .@�m� -   $��Создать ZIP�� �� $.��(free-icon-lightning-bolt-3325155 (2).png��m� -   $��Извлечь ZIP�� �� $"��free-icon-zipper-2088946.png��m� -   $"��Свернуть в трей�� �� $"��free-icon-reduce-5068508.png��m� -   $��
Выход�� �� $"��free-icon-logout-5087631.png��.��� -    �Name$ 
contextRow�� � RowDefinitions �� $��25 ��� $��35 ��� $��* �.+��� -    ��Name$ rect1�����	#FF36418B$��0��� $	��5,0q�.+��� -    ��Name$
 btn1��$��Файл��$��2q���Z� ��Z� �� -	   $
 btn2��$��Настройки��$��2q���Z� ��Z� �� -
   $
 btn4��$��В трей��$��2q���Z� ��Z� �� -   $
 btn3��$��О приложении��$��2q���Z� ��Z� � $��1��.+��� -    ��Name$ chBox2��$��12(�$"��Загрузить на Mega��$��10q��� $��2�����	#FF4D5AAC$��10q�.@��� -    ��Name$
 tab1��$��Создать ZIP��.��� $��10q�$��2�� �� $��50 ��� $��* �.+��� -   	 ��Name$	 text1��$
��Bold"�$��15(�$��10q�
 �� TextAlignment$
 Center=�$@��:Перетащите в область для сжатия���� $��1��$)��#Resources/free-icon-zip-1112204.png��$��10q��� -   
���   �$��2q� �� CornerRadius$ 10��$��2�����	#004D5AAC J� 	AllowDrop . �� -   $
 tab2��$��Извлечь ZIP��.��� $��10q�$��2�� �� $��50 ��� $��* �.+��� -   $	 text2��$
��Bold"�$��15(�$��10q�$
 Center=�$H��BПеретащите в область для извлечения���� $��1��$)��#Resources/free-icon-zip-1112204.png��$��10q��� -   
���   �$��2q�$ 10��$��2�����	#004D5AAC . 