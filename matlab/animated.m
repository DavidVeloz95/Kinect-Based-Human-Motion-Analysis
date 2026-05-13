clc;
clear;
close all;

% Lee el archivo
file = 5;            % Which file?
D = 2;                  % 2D view or 3D?
if file == 1
    data = dlmread('Frei_Proband1.txt', ',');
elseif file == 2
    data = dlmread('SAS_Proband1_1.txt', ',');
elseif file == 3
    data = dlmread('SAS_Proband1_2.txt', ',');
elseif file == 4
    data = dlmread('SAS_Proband1_3.txt', ',');
elseif file == 5
    data = dlmread('POCO_Proband1.txt', ',');
elseif file == 6
    data = dlmread('SIP_Proband1.txt', ',');
end

contador = 1;
w = 1;
% Extrae las columnas
Frame = data(:, 1);

% Encuentra los Frames únicos
uniqueFrames = unique(Frame);

for i=1:length(data)
    if data(i,1) ~= uniqueFrames(w,1)
        contador = data(i,1);
        w = w+1;
    end
    if data(i,2) == 0
        SpineBase(contador,:) = data(i,:);
    elseif data(i,2) == 1
        SpineMid(contador,:) = data(i,:);
    elseif data(i,2) == 2
        Neck(contador,:) = data(i,:);
    elseif data(i,2) == 3
        Head(contador,:) = data(i,:);
    elseif data(i,2) == 4
        ShoulderLeft(contador,:) = data(i,:);
    elseif data(i,2) == 5
        ElbowLeft(contador,:) = data(i,:);
    elseif data(i,2) == 6
        WristLeft(contador,:) = data(i,:);
    elseif data(i,2) == 7
        HandLeft(contador,:) = data(i,:);
    elseif data(i,2) == 8
        ShoulderRight(contador,:) = data(i,:);
    elseif data(i,2) == 9
        ElbowRight(contador,:) = data(i,:);
    elseif data(i,2) == 10
        WristRight(contador,:) = data(i,:);
    elseif data(i,2) == 11
        HandRight(contador,:) = data(i,:);
    elseif data(i,2) == 12
        HipLeft(contador,:) = data(i,:);
    elseif data(i,2) == 13
        KneeLeft(contador,:) = data(i,:);
    elseif data(i,2) == 14
        AnkleLeft(contador,:) = data(i,:);
    elseif data(i,2) == 15
        FootLeft(contador,:) = data(i,:);
    elseif data(i,2) == 16
        HipRight(contador,:) = data(i,:);
    elseif data(i,2) == 17
        KneeRight(contador,:) = data(i,:);
    elseif data(i,2) == 18
        AnkleRight(contador,:) = data(i,:);
    elseif data(i,2) == 19
        FootRight(contador,:) = data(i,:);
    elseif data(i,2) == 20
        SpineShoulder(contador,:) = data(i,:);
    elseif data(i,2) == 21
        HandTipLeft(contador,:) = data(i,:);
    elseif data(i,2) == 22
        ThumbLeft(contador,:) = data(i,:);
    elseif data(i,2) == 23
        HandTipRight(contador,:) = data(i,:);
    elseif data(i,2) == 24
        ThumbRight(contador,:) = data(i,:);
    end 
end
% HandRight =[];

body = {SpineBase, SpineMid, Neck, Head, ShoulderLeft, ElbowLeft,...
    WristLeft, HandLeft, ShoulderRight, ElbowRight, WristRight,...
    HandRight, HipLeft, KneeLeft, AnkleLeft, FootLeft, HipRight,...
    KneeRight, AnkleRight, FootRight, SpineShoulder, HandTipLeft,...
    ThumbLeft, HandTipRight, ThumbRight};

% minLength = 1000;
% 
% for i = 1:length(body)
%     currentLength = length(body{i});
%     if currentLength < minLength
%         minLength = currentLength;
%     end
% end

figure1 = figure(1);
for fr = 1:length(body{1})
    frameData = data(Frame == fr, :);
    if D == 2
        % 2D
        f = scatter(frameData(:, 3), frameData(:, 4), 'filled');
        axis([[-1 1], [-1 1.5]]);
    elseif D == 3
        % 3D
        f = scatter3(frameData(:, 3), frameData(:, 5), frameData(:, 4), 'filled');
        axis([[-1 1], [-1 8], [-1 1.5]]);
    end
    % Guardar la imagen como PNG
    filename = sprintf('frame_%d.png', fr);  % Nombre del archivo
    saveas(f, filename);  % Guardar el gráfico como PNG
    pause(0.01);
end
title('Proband'); xlabel('X'); ylabel('Y');



% clc
% l = 50;
% (abs(ShoulderRight(l,3) - FootRight(l,3))) + (abs(ShoulderLeft(l,3) - FootLeft(l,3)))/2
% figure(1);
% frameData = data(Frame == l, :);
% if D == 2
%     % 2D
%     scatter(frameData(:, 3), frameData(:, 4), 'filled');
%     axis([[-1 1], [-1 1.5]]);
% elseif D == 3
%     % 3D
%     scatter3(frameData(:, 3), frameData(:, 5), frameData(:, 4), 'filled');
%     axis([[-1 1], [-1 8], [-1 1.5]]);
% end
% title('Proband'); xlabel('X'); ylabel('Y');
