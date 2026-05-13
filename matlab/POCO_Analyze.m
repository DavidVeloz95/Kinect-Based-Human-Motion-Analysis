function [t_ende, EO_m_pitch, EO_m_roll, EO_g_pitch, EO_g_roll, EC_m_pitch, EC_m_roll, EC_g_pitch, EC_g_roll] = POCO_Analyze(body, qty_frames)

t_ende = round(qty_frames/30);

% Stance with closed feet and open and closed eyes
A_L_x = body{1,15}(:,3); A_L_y = body{1,15}(:,4);
A_R_x = body{1,19}(:,3); A_R_y = body{1,19}(:,4);

% Tomar los primeros 600 valores y promediarlos
O1 = [body{1,1}((1:600),3), body{1,1}((1:600),4), body{1,1}((1:600),5)];
mO1 = mean(O1);

% Determinar el punto de cruce de los ejes vertical, sagittal y frontal
x0 = mean(A_L_x+((A_R_x-A_L_x)/2));
y0 = mean(A_L_y+((A_R_y-A_L_y)/2));
z0 = mO1(3);

% Calcular el angulo para cada frame
P1 = [body{1,1}(:,3)-x0, body{1,1}(:,4)-y0, body{1,1}(:,5)-z0];
P2 = [body{1,17}(:,3)-x0, body{1,17}(:,4)-y0, body{1,17}(:,5)-z0];
P3 = [body{1,13}(:,3)-x0, body{1,13}(:,4)-y0, body{1,13}(:,5)-z0];
P4 = [body{1,2}(:,3)-x0, body{1,2}(:,4)-y0, body{1,2}(:,5)-z0];
P5 = [body{1,21}(:,3)-x0, body{1,21}(:,4)-y0, body{1,21}(:,5)-z0];

for i=1:qty_frames
    pitch(i) = rad2deg(atan(P1(i,3)/P1(i,2)));
    roll(i) = rad2deg(atan(P1(i,1)/P1(i,2)));
end

% Separar entre datos con ojos abiertos y cerrados
EO_pitch = pitch(1:600)';
EO_roll = roll(1:600)';
EC_pitch = pitch(601:1200)';
EC_roll = roll(601:1200)';

% Calcular el promedio de los números positivos y negativos de pitch
EO_pitch_pos = EO_pitch(EO_pitch > 0);
EO_pitch_neg = EO_pitch(EO_pitch < 0);
EO_mean_pp = mean(EO_pitch_pos);
EO_mean_pn = mean(EO_pitch_neg);

EC_pitch_pos = EC_pitch(EC_pitch > 0);
EC_pitch_neg = EC_pitch(EC_pitch < 0);
if isempty(EC_pitch_pos)
    EC_pitch_pos = 0;
end
if isempty(EC_pitch_neg)
    EC_pitch_neg = 0;
end
EC_mean_pp = mean(EC_pitch_pos);
EC_mean_pn = mean(EC_pitch_neg);

% Calcular el promedio de los números positivos y negativos de roll
EO_roll_pos = EO_roll(EO_roll > 0);
EO_roll_neg = EO_roll(EO_roll < 0);
if isempty(EO_roll_neg)
    EO_roll_neg = 0;
end
if isempty(EO_roll_pos)
    EO_roll_pos = 0;
end
EO_mean_rp = mean(EO_roll_pos);
EO_mean_rn = mean(EO_roll_neg);

EC_roll_pos = EC_roll(EC_roll > 0);
EC_roll_neg = EC_roll(EC_roll < 0);
if isempty(EC_roll_neg)
    EC_roll_neg = 0;
end
if isempty(EC_roll_pos)
    EC_roll_pos = 0;
end
EC_mean_rp = mean(EC_roll_pos);
EC_mean_rn = mean(EC_roll_neg);

% Calcular mean pitch, mean roll y velocidad de ambos:
EO_m_pitch = EO_mean_pp - EO_mean_pn;
EO_g_pitch = EO_m_pitch / 20;
EO_m_roll = EO_mean_rp - EO_mean_rn;
EO_g_roll = EO_m_roll / 20;

EC_m_pitch = EC_mean_pp - EC_mean_pn;
EC_g_pitch = EC_m_pitch / 20;
EC_m_roll = EC_mean_rp - EC_mean_rn;
EC_g_roll = EC_m_roll / 20;

fprintf('Testzeit [s]: %0.4f \n', t_ende);
fprintf('\n');

fprintf('EO Deflection Range pitch [°]: %0.4f \n', EO_m_pitch);
fprintf('EO Deflection Range roll [°]: %0.4f \n', EO_m_roll);
fprintf('EO Mean Sway Velocity pitch [°/s]: %0.4f \n', EO_g_pitch);
fprintf('EO Mean Sway Velocity roll [°/s]: %0.4f \n', EO_g_roll);

fprintf('EC Deflection Range pitch [°]: %0.4f \n', EC_m_pitch);
fprintf('EC Deflection Range roll [°]: %0.4f \n', EC_m_roll);
fprintf('EC Mean Sway Velocity pitch [°/s]: %0.4f \n', EC_g_pitch);
fprintf('EC Mean Sway Velocity roll [°/s]: %0.4f \n', EC_g_roll);

end