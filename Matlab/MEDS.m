N = [57,58]; %���������� ����� (������ ��� �������������� � ������ �����), ������� �� ����� ����������� � ����.
rays = 7; %���������� �����
K = 1000;  %����� ����� k �������� ������� 
mu = zeros(rays, 2, K); %������� �������������

%������� ���������
power = [0 -1 -2 -3 -8 -17.2 -20.8];
sigma = sqrt(10.^(power./10)); % �������� ���������

%����� ������������, �������� MEDS

%������� ������������ ��������
c = createGainsByMEDS(sigma, N(1));

%������� ������� ������������� ��������

%fray=800*10^6; %������� ����. (��)
%fmax = fray/(3*10^8); %������������ ������������ ��������
fmax = 300;
f = createFrequenciesByMEDS(fmax, N(1));

% ������� ����
teta = createPhasesByMEDS(N(1));

%������� ������������� 
%Ts = 5000*10^(-6); 
Ts=1/5000;

for z=1:rays
    %������� �������������� ����� � ������ ����� ��������
    for i=1:2
        for k=1:K
            for n=1:N(i)
                temp=c(i,z)*cos(2*pi*f(i,n)*k*Ts+teta(i,n));
                mu(z,i,k) = mu(z,i,k)+temp;
            end
        end
    end
end


x=squeeze(mu(1,1,:)+1j*mu(1,2,:));

 M=30;
 R=correlation(x,M);

 x1=squeeze(mu(1,1,:));
 x2=squeeze(mu(1,2,:));

% R2=correlation(x1,M);

% RR =  correlation(x1,M)+correlation(x2,M);
 hold off;
 plot(correlation(x1,M));
 hold on;
 plot(correlation(x2,M));
 plot(real(R(1))*besselj(0,2*pi*fmax*Ts*(0:M)),'r-.');



