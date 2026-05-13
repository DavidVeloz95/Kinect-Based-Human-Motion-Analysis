%% Aufräumen %%
clc;
clear;
close all;

%% Data %%
% Lee el archivo
file = 1;            % Which file?
D = 2;                  % 2D view or 3D?
if file == 1
    data = dlmread('SAS_Proband1_1.txt', ',');    
elseif file == 2
    data = dlmread('SAS_Proband1_2.txt', ',');
elseif file == 3
    data = dlmread('SAS_Proband1_3.txt', ',');
elseif file == 4
    data = dlmread('POCO_Proband1.txt', ',');
elseif file == 5
    data = dlmread('SIP_Proband1.txt', ',');
end

qty_frames = 1;
w = 1;
% Extrae las columnas
Frame = data(:, 1);

% Encuentra los Frames únicos
uniqueFrames = unique(Frame);

for i=1:length(data)
    if data(i,1) ~= uniqueFrames(w,1)
        qty_frames = data(i,1);
        w = w+1;
    end
    if data(i,2) == 0
        SpineBase(qty_frames,:) = data(i,:);
    elseif data(i,2) == 1
        SpineMid(qty_frames,:) = data(i,:);
    elseif data(i,2) == 2
        Neck(qty_frames,:) = data(i,:);
    elseif data(i,2) == 3
        Head(qty_frames,:) = data(i,:);
    elseif data(i,2) == 4
        ShoulderLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 5
        ElbowLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 6
        WristLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 7
        HandLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 8
        ShoulderRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 9
        ElbowRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 10
        WristRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 11
        HandRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 12
        HipLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 13
        KneeLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 14
        AnkleLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 15
        FootLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 16
        HipRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 17
        KneeRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 18
        AnkleRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 19
        FootRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 20
        SpineShoulder(qty_frames,:) = data(i,:);
    elseif data(i,2) == 21
        HandTipLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 22
        ThumbLeft(qty_frames,:) = data(i,:);
    elseif data(i,2) == 23
        HandTipRight(qty_frames,:) = data(i,:);
    elseif data(i,2) == 24
        ThumbRight(qty_frames,:) = data(i,:);
    end 
end

body = cell(2, 25);

body(1,:) = {SpineBase, SpineMid, Neck, Head, ShoulderLeft, ElbowLeft,...
    WristLeft, HandLeft, ShoulderRight, ElbowRight, WristRight,...
    HandRight, HipLeft, KneeLeft, AnkleLeft, FootLeft, HipRight,...
    KneeRight, AnkleRight, FootRight, SpineShoulder, HandTipLeft,...
    ThumbLeft, HandTipRight, ThumbRight};

% Definir los nombres de las partes del cuerpo
nombresPartesCuerpo = ["SpineBase", "SpineMid", "Neck", "Head", "ShoulderLeft", "ElbowLeft",...
    "WristLeft", "HandLeft", "ShoulderRight", "ElbowRight", "WristRight",...
    "HandRight", "HipLeft", "KneeLeft", "AnkleLeft", "FootLeft", "HipRight",...
    "KneeRight", "AnkleRight", "FootRight", "SpineShoulder", "HandTipLeft",...
    "ThumbLeft", "HandTipRight", "ThumbRight"];

% Asignar los nombres a la segunda fila de la celda 'body'
body(2, :) = cellstr(nombresPartesCuerpo);

%% SAS Verarbeitung %%
if file == 1 || file == 2 || file == 3
    [t_ende, AW, h, U_DR_ML, U_DR_AP, U_RoM_AP_RH_f, U_RoM_AP_LH_f, D_DR_ML, D_DR_AP] = SAS_Analyze(body, qty_frames);
end

%% POCO Verarbeitung %%
if file == 4
    [t_ende, EO_m_pitch, EO_m_roll, EO_g_pitch, EO_g_roll, EC_m_pitch, EC_m_roll, EC_g_pitch, EC_g_roll] = POCO_Analyze(body, qty_frames);
end

%% SIP Verarbeitung %%
if file == 5
    [t_ende, RoM_K_R, RoM_K_L, Cadence_R, Cadence_L] = SIP_Analyze(body, qty_frames);
end
%% Aufräumen %%
clear SpineBase SpineMid Neck Head ShoulderLeft ElbowLeft...
    WristLeft HandLeft ShoulderRight ElbowRight WristRight...
    HandRight HipLeft KneeLeft AnkleLeft FootLeft HipRight...
    KneeRight AnkleRight FootRight SpineShoulder HandTipLeft...
    ThumbLeft HandTipRight ThumbRight data Frame uniqueFrames i w...
    nombresPartesCuerpo;