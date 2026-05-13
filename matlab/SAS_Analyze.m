function [t_ende, AW, h, U_DR_ML, U_DR_AP, U_RoM_AP_RH_f, U_RoM_AP_LH_f, D_DR_ML, D_DR_AP] = SAS_Analyze(body, qty_frames)

% Zeit
t_ende = round(qty_frames/30);

% Anzahl der Wiederholungen
AW = 5;

y = body{1,1}(:,4);

% Establecer un umbral
th = -abs(min(y)) + ((abs(max(y)) + abs(min(y)))*0.5);
[picos, ubicaciones] = findpeaks(y, 'MinPeakHeight', th, 'MinPeakDistance',80);
w = 0;
d = diff(ubicaciones);
u = [];
p = [];
for i=1:length(d)
    if d(i) > 250
        u(i) = ubicaciones(i);
        u(i+1) = ubicaciones(i) + 70;
        w = w + 1;
        p(i) = picos(i);
        p(i+1) = y(u(i+1));
    else
        u(i+w) = ubicaciones(i);
    end
end
if ~isempty(p)
    ubicaciones = u';
    picos = p';
end

% picos = [-0.0464; -0.1021; -0.0847; -0.1025; -0.0831; -0.1266; -0.0699; -0.1226; -0.0960; -0.1055];
% ubicaciones = [105; 175; 361; 476; 660; 773; 957; 1079; 1256; 1341];
th1 = -abs(min(-y)) + ((abs(max(-y)) + abs(min(-y)))*2/3);
[picos1, ubicaciones1] = findpeaks(-y, 'MinPeakHeight', th1, 'MinPeakDistance',100);

if length(ubicaciones)>10
    ubicaciones(end)=[];
    if length(ubicaciones)>10
        ubicaciones(end)=[];
        if length(ubicaciones)>10
            ubicaciones(end)=[];
        end
    end
end

if length(ubicaciones1)>10
    ubicaciones1(end)=[];
    picos1(end)=[];
    if length(ubicaciones1)>10
        ubicaciones1(end)=[];
        picos1(end)=[];
        if length(ubicaciones1)>10
            ubicaciones1(end)=[];
            picos1(end)=[];
            if length(ubicaciones1)>10
                ubicaciones1(end)=[];
                picos1(end)=[];
            end
        end
    end
end

%% Durchschnittliche Verschiebung

% Anpassung einer polynomialen Trendlinie vom Grad 2
p = polyfit(ubicaciones1, picos1, 2);       % Polynomialkoeffizienten
% Erstellung die Gleichung der Trendlinie
syms x_symbol;                              % Erstellung ein Symbol für x
trend_equation_R = poly2sym(p, x_symbol);   % Gleichung der Trendlinie
% Nutzung der Trendlinie für die Verbesserung der Daten
t = 1:qty_frames;             % Punkte zum Zeichnen der Trendlinie
y_R = polyval(p, t);          % Berechnung der y-Werte für die Punkte x
% Neue normalisierte Daten
n_y = y+y_R'-0.03;

% Establecer un umbral para la senal normalizada
th = -abs(min(n_y)) + ((abs(max(n_y)) + abs(min(n_y)))*2/3);
[n_picos, ~] = findpeaks(n_y, 'MinPeakHeight', th, 'MinPeakDistance',90);
th1 = -abs(min(-n_y)) + ((abs(max(-n_y)) + abs(min(-n_y)))*2/3);
[picos1, ubicaciones1] = findpeaks(-n_y, 'MinPeakHeight', th1, 'MinPeakDistance',90);
% picos1(1)=[];
% ubicaciones1(1)=[];

if length(ubicaciones1)>10
    ubicaciones1(end)=[];
    picos1(end)=[];
    if length(ubicaciones1)>10
        ubicaciones1(end)=[];
        picos1(end)=[];
        if length(ubicaciones1)>10
            ubicaciones1(end)=[];
            picos1(end)=[];
            if length(ubicaciones1)>10
                ubicaciones1(end)=[];
                picos1(end)=[];
            end
        end
    end
end

% ubicaciones1 = [80; 179; 363; 484; 695; 786; 972; 1078; 1265; 1376];
h = mean(n_picos);

%% Up and Down - Zeit
Zeit = (ubicaciones - ubicaciones1)/30; % in Sekunden
Zeit(end) = [];

figure(1); hold on; grid on;
plot(t, y, 'g', ubicaciones, y(ubicaciones), 'ro', ubicaciones1, y(ubicaciones1), 'bo');

% Up -> Down
U2D = mean(Zeit(Zeit>0));
% Down -> Up
D2U = mean(abs(Zeit(Zeit<0)));

% Revisar que haya aunque sea un valor de Zeit negativo

%% Deflection range ML und AP

x = body{1,1}(:,3);
z = body{1,1}(:,5);

if n_y(ubicaciones(1)) > n_y(ubicaciones1(1))
    D = [1, 3, 5, 7, 9];
    U = [2, 4, 6, 8, 10];
else
    D = [2, 4, 6, 8, 10];
    U = [1, 3, 5, 7, 9];
end
% Down
D_DR_ML = [ x(ubicaciones1(D(1))) - x(ubicaciones(D(1))) ; x(ubicaciones1(D(2))) - x(ubicaciones(D(2))); ...
    x(ubicaciones1(D(3))) - x(ubicaciones(D(3))); x(ubicaciones1(D(4))) - x(ubicaciones(D(4)));...
    x(ubicaciones1(D(5))) - x(ubicaciones(D(5)))];
D_DR_ML_f = mean( D_DR_ML )*100;

D_DR_AP = [ z(ubicaciones1(D(1))) - z(ubicaciones(D(1))) ; z(ubicaciones1(D(2))) - z(ubicaciones(D(2))); ...
    z(ubicaciones1(D(3))) - z(ubicaciones(D(3))); z(ubicaciones1(D(4))) - z(ubicaciones(D(4)));...
    z(ubicaciones1(D(5))) - z(ubicaciones(D(5)))];
D_DR_AP_f = mean( D_DR_AP )*100;

% UP
U_DR_ML = [ x(ubicaciones1(U(1))) - x(ubicaciones(U(1))) ; x(ubicaciones1(U(2))) - x(ubicaciones(U(2))); ...
    x(ubicaciones1(U(3))) - x(ubicaciones(U(3))); x(ubicaciones1(U(4))) - x(ubicaciones(U(4)));...
    x(ubicaciones1(U(5))) - x(ubicaciones(U(5)))];
U_DR_ML_f = mean( U_DR_ML )*100;

U_DR_AP = [ z(ubicaciones1(U(1))) - z(ubicaciones(U(1))) ; z(ubicaciones1(U(2))) - z(ubicaciones(U(2))); ...
    z(ubicaciones1(U(3))) - z(ubicaciones(U(3))); z(ubicaciones1(U(4))) - z(ubicaciones(U(4)));...
    z(ubicaciones1(U(5))) - z(ubicaciones(U(5)))];
U_DR_AP_f = mean( U_DR_AP )*100;

%% Hände %%
z_RH = body{1,12}(:,5);
z_LH = body{1,8}(:,5);

U_RoM_AP_RH = [ z_RH(ubicaciones1(U(1))) - z_RH(ubicaciones(U(1))) ; z_RH(ubicaciones1(U(2))) - z_RH(ubicaciones(U(2))); ...
    z_RH(ubicaciones1(U(3))) - z_RH(ubicaciones(U(3))); z_RH(ubicaciones1(U(4))) - z_RH(ubicaciones(U(4)));...
    z_RH(ubicaciones1(U(5))) - z_RH(ubicaciones(U(5)))];
U_RoM_AP_RH_f = mean( U_RoM_AP_RH )*100;

U_RoM_AP_LH = [ z_LH(ubicaciones1(U(1))) - z_LH(ubicaciones(U(1))) ; z_LH(ubicaciones1(U(2))) - z_LH(ubicaciones(U(2))); ...
    z_LH(ubicaciones1(U(3))) - z_LH(ubicaciones(U(3))); z_LH(ubicaciones1(U(4))) - z_LH(ubicaciones(U(4)));...
    z_LH(ubicaciones1(U(5))) - z_LH(ubicaciones(U(5)))];
U_RoM_AP_LH_f = mean( U_RoM_AP_LH )*100;


%% Ergebnise %%

fprintf('Testzeit [s]: %d \n', t_ende);
fprintf('Anzahl der Wiederholungen: 5 \n');
fprintf('Durchschnittliche Verschiebung [m]: %0.4f \n', h);
fprintf('\n');

fprintf('Up - Zeit [s]: %0.4f \n', D2U);
% fprintf('Up - Geschwindigkeit [m/s]: %0.4f \n', ges_sit2stand);
fprintf('Up Deflection Range Medio-Lateral [cm]: %0.4f \n', U_DR_ML_f);
fprintf('Up Deflection Range Anterior-Posterior [cm]: %0.4f \n', U_DR_AP_f);
fprintf('Up Range of motion right hand [cm]: %0.4f \n', U_RoM_AP_RH_f);
fprintf('Up Range of motion left hand [cm]: %0.4f \n', U_RoM_AP_LH_f);

fprintf('Down - Zeit [s]: %0.4f \n', U2D);
% fprintf('Down - Geschwindigkeit [m/s]: %0.4f \n', ges_stand2sit);
fprintf('Down deflection range Medio-Lateral [cm]: %0.4f \n', D_DR_ML_f);
fprintf('Down deflection range Anterior-Posterior [cm]: %0.4f \n', D_DR_AP_f);

% clear a g k m n o p picos picos1 picos_filtrados picos_filtrados1...
%     r sit stand ubicaciones ubicaciones1 ubicaciones_filtradas...
%     ubicaciones_filtradas1 umbral umbral1;
end