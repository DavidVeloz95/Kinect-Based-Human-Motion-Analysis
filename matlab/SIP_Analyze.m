function [t_ende, RoM_K_R, RoM_K_L, Cadence_R, Cadence_L] = SIP_Analyze(body, qty_frames)

t_ende = round(qty_frames/30);

% Stance with closed feet and open and closed eyes
K_L = body{1,18}(:,4);                              % K_L-Signal
K_R = body{1,14}(:,4);                              % K_R-Signal

% Filter - gleitenden Durchschnitt
windowSize = 3;                                     % Filter Fenstergröße
filtro = ones(1, windowSize) / windowSize;          % Filterkoeffizienten
K_L = conv(K_L, filtro, 'same');                    % gefilterte K_L
K_R = conv(K_R, filtro, 'same');                    % gefilterte K_R

% FindPeaks-Funktion
th_R = mean(-K_R)-0.03;
th_L = mean(-K_L)-0.03;
[u_peaks_R, u_locs_R] = findpeaks(-K_R, 'MinPeakHeight', th_R);
[u_peaks_L, u_locs_L] = findpeaks(-K_L, 'MinPeakHeight', th_L);

% Anpassung einer polynomialen Trendlinie vom Grad 2
p_R = polyfit(u_locs_R, u_peaks_R, 2);      % Polynomialkoeffizienten
p_L = polyfit(u_locs_L, u_peaks_L, 2);      % Polynomialkoeffizienten

% Erstellung die Gleichung der Trendlinie
syms x_symbol;                              % Erstellung ein Symbol für x
trend_equation_R = poly2sym(p_R, x_symbol); % Gleichung der Trendlinie
trend_equation_L = poly2sym(p_L, x_symbol); % Gleichung der Trendlinie

% Nutzung der Trendlinie für die Verbesserung der Daten
x = 1:qty_frames;               % Punkte zum Zeichnen der Trendlinie
y_R = polyval(p_R, x);          % Berechnung der y-Werte für die Punkte x
y_L = polyval(p_L, x);          % Berechnung der y-Werte für die Punkte x

% Neue normalisierte Daten
n_K_L = K_L+y_L';
n_K_R = K_R+y_R';
th_R = mean(n_K_R);
th_L = mean(n_K_L);

% findpeaks(n_K_L, 'MinPeakHeight', th_L)
[peaks_R, locs_R] = findpeaks(n_K_R, 'MinPeakHeight', th_R);
[peaks_L, locs_L] = findpeaks(n_K_L, 'MinPeakHeight', th_L);

q_R = find(diff(locs_R)>100);
E_T_R = abs(round(locs_R(1)/30)*30 - (round(locs_R(q_R(1))/30)+1)*30)/30;
Z_T_R = abs((round(locs_R(q_R(1)+1)/30)-1)*30 - (round(locs_R(q_R(2))/30)+1)*30)/30;
D_T_R = abs( ((round(locs_R(q_R(2)+1)/30)-1)*30) - ((round(locs_R(end)/30)+1)*30) )/30;

q_L = find(diff(locs_L)>100);
E_T_L = abs(round(locs_L(1)/30)*30 - (round(locs_L(q_L(1))/30)+1)*30)/30;
Z_T_L = abs((round(locs_L(q_L(1)+1)/30)-1)*30 - (round(locs_L(q_L(2))/30)+1)*30)/30;
D_T_L = abs( ((round(locs_L(q_L(2)+1)/30)-1)*30) - ((round(locs_L(end)/30)+1)*30) )/30;

RoM_K_R = mean(peaks_R);
RoM_K_L = mean(peaks_L);

Steps_R = length(peaks_R);
Steps_L = length(peaks_L);

Cadence_R = Steps_R / ((E_T_R + Z_T_R + D_T_R)/60);
Cadence_L = Steps_L / ((E_T_L + Z_T_L + D_T_L)/60);

fprintf('Testzeit [s]: %d \n', t_ende);
fprintf('\n');

fprintf('Range of Motion Knee L [cm]: %0.4f \n', RoM_K_L);
fprintf('Range of Motion Knee R [cm]: %0.4f \n', RoM_K_R);
fprintf('Cadence L [steps/min]: %0.4f \n', Cadence_L);
fprintf('Cadence R [steps/min]: %0.4f \n', Cadence_R);

end