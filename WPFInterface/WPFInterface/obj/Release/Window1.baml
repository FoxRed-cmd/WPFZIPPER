   M S B A M L   `   `   `  ���� G  CWPFInterface, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null+clr-namespace:WPFInterfaceWPFInterface      WPFInterface.Window1   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     @d2http://schemas.microsoft.com/expression/blend/2008     Jmc;http://schemas.openxmlformats.org/markup-compatibility/2006     ,localclr-namespace:WPFInterface     -     � Title$  Натсройки��$	��450��$��550     �����	#FF4D5AAC � 
ResizeMode$ NoResize=� � WindowStyle$ 
ToolWindow=�.���  � ColumnDefinitions �� $	��45* ��� $
��181* ��� $	��46* � � RowDefinitions �� $
��338* ��� $	��51* ��� $	��33* �.+��� ���	#FF4D5AAC$��10q�$��3��$��2��.@��� $��
Mega Drive��.���� .+��� -    ��Name$ label3��$��Логин: ��$
��5, 0q��� -    ��Name$
 txt3��$��10, 0q��� -   $ label4��$��Пароль: ��$
��5, 0q�G� -    G�Name$
 txt4��$��10, 0q��� -   $ label5��$2��,Ссылка на папку в Mega Drive: ��$
��5, 0q��� -   $
 txt5��$��10, 0q��� -    ��Name$ 	txtblock1��$��Center=�$��10q��� -	   	 ��Name$	 link1�����	#FF0BD8D8$��https://mega.nz/start?�.�%#Регистрация на Mega.io�� $-��'Настройки приложения��.���  �� $
��104* ��� $
��198* ��� $	��39* �.+��� -
   
 ��Name$
 gbox1��$��Уведомления��$��10q�.���� .+�	� -    	�Name$	 rb1��$��10q�$���zПолучать в центре уведомлений (не включает уведомления об ошибках)��	� -   $	 rb2��$��10, 0q�$;��5Получать уведомления в MessageBox���� -   $
 gbox2��$��1��$��Сохранение��$��10q�.���  �� $
��205* ��� $	��38* �.+��� $��0��.+�	� -   $	 rb3��$��10q�$f��`Указывать путь во время создания (извлечения) архива��	� -   $	 rb4��$��10, 0q�$E��?Использовать путь указанный здесь���� $��5,0,10,0q�$4��.Путь к сохранению архива:���� -   $ defZIP��$��	10,0,10,0q��� $��5,0,10,0q�$4��.Путь к извлечению архива:���� -   $ defEXT��$��	10,0,10,0q��� $��1��.+��� -    ��Name$
 btn2��$��
Обзор��$��10,74,10,15q��� -   $
 btn3��$��
Обзор��$��10q��� -   $
 btn1��$��2��$��Применить��$��
10,0,10,10q�$��2���� -   $
 btn5��$��2��$��
Сброс��$��
10,0,10,10q�$��0��