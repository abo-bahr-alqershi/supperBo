// frontend/src/components/Home/Sections/CompactPropertyList.tsx
import React from 'react';
import {
  Box,
  List,
  ListItem,
  ListItemText,
  ListItemAvatar,
  Avatar,
  Typography,
  Chip,
  Divider,
} from '@mui/material';
import { DynamicHomeSection } from '../../../types/homeSections.types';
import SectionHeader from '../Common/SectionHeader';
import { LocationOn, Star } from '@mui/icons-material';
import { useNavigate } from 'react-router-dom';

interface CompactPropertyListProps {
  section: DynamicHomeSection;
}

const CompactPropertyList: React.FC<CompactPropertyListProps> = ({ section }) => {
  const navigate = useNavigate();
  
  const properties = section.content
    .filter(item => item.contentType === 'PROPERTY')
    .map(item => item.data);

  if (properties.length === 0) {
    return null;
  }

  return (
    <Box>
      <SectionHeader
        title={section.title}
        subtitle={section.subtitle}
      />
      
      <List sx={{ bgcolor: 'background.paper', borderRadius: 2 }}>
        {properties.map((property, index) => (
          <React.Fragment key={property.id || index}>
            <ListItem
              alignItems="flex-start"
              sx={{
                cursor: 'pointer',
                '&:hover': { bgcolor: 'grey.50' },
              }}
              onClick={() => navigate(`/properties/${property.id}`)}
            >
              <ListItemAvatar>
                <Avatar
                  variant="rounded"
                  src={property.mainImageUrl || property.images?.[0]}
                  sx={{ width: 80, height: 80, mr: 2 }}
                />
              </ListItemAvatar>
              
              <ListItemText
                primary={
                  <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
                    <Typography variant="h6" component="span">
                      {property.name}
                    </Typography>
                    {property.basePrice && (
                      <Typography variant="h6" color="primary">
                        {property.basePrice.toLocaleString()} {property.currency}
                      </Typography>
                    )}
                  </Box>
                }
                secondary={
                  <Box>
                    <Box sx={{ display: 'flex', alignItems: 'center', mt: 0.5 }}>
                      <LocationOn sx={{ fontSize: 16, mr: 0.5 }} />
                      <Typography variant="body2" component="span">
                        {property.city}
                      </Typography>
                      
                      {property.averageRating && (
                        <>
                          <Star sx={{ fontSize: 16, ml: 2, mr: 0.5, color: 'warning.main' }} />
                          <Typography variant="body2" component="span">
                            {property.averageRating}
                          </Typography>
                        </>
                      )}
                    </Box>
                    
                    {property.amenities && (
                      <Box sx={{ mt: 1, display: 'flex', gap: 0.5, flexWrap: 'wrap' }}>
                        {property.amenities.slice(0, 3).map((amenity: string, idx: number) => (
                          <Chip key={idx} label={amenity} size="small" />
                        ))}
                      </Box>
                    )}
                  </Box>
                }
              />
            </ListItem>
            {index < properties.length - 1 && <Divider variant="inset" component="li" />}
          </React.Fragment>
        ))}
      </List>
    </Box>
  );
};

export default CompactPropertyList;