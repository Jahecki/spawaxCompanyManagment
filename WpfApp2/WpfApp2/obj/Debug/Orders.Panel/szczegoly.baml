   M S B A M L   `   `   `  ����  WpfApp2"    WpfApp2.Orders.Panel.Window1   5   	   R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35\ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	     � Title$  	Szczegoly��5   	   $	��200��6   $	��800��6(   .��5      � .+�5   
   &   System.Windows.Controls.DataGrid  -   5          Name$ SzczegolyDataGrid��5         AutoGenerateColumns .  $��Stretch=�5      $��Stretch=�5	      $��10q�5
         RowBackground
 �����   AlternatingRowBackground
 �����   GridLinesVisibility$ 
Horizontal=�5         ColumnHeaderHeight$ 30Q�5         	RowHeight$ 40Q�5      
�������$��1q�5       � 	Resources 5      =  7System.Windows.Controls.Primitives.DataGridColumnHeader	 �� 
TargetType
 �� Property �� Value��%  '        �� 5      "	  6   .�5      �� 
 � ��
 �<��63   �� 5      
 � ��
 �����63   �� 5      
 � ��$
 Bold"�63   �� 5      
 � ��$ 14(�61   �� 5      
 � ��$
 Left=�6C   �� 5      
 � ��$ Stretch=�6A   5      5         Columns 5!      0  *System.Windows.Controls.DataGridTextColumn  5#      ,  &System.Windows.Controls.DataGridColumn   Header$ ID��6%      Width5  /System.Windows.Controls.DataGridLengthConverter$	 100 6V      
IsReadOnly . 1  +System.Windows.Controls.DataGridBoundColumn   Binding 61   �� *Numer_zamowienia+6b     5&      $ Nazwa Produktu��6%   $ * 6`    6=   �� *Nazwa_produktu+6`     5)      $ Kolor��6%   $ * 6N    64   �� *Kolor+6N     5,      $ Ilość materiałów��6%   $ * 6d    6?   �� *Ilosc_materialow+6d     5/      $ Cena za materiały��6%   $ * 6f    6@   �� *Cena_za_materialy+6f     52      $ Cena końcowa��6%   $ * 6\    6;   �� *Cena_koncowa+6\   54      55      56      